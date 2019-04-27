namespace SmartRecipes.Api

module Generic =
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open FSharpPlus.Data
    open FSharp.Control.Tasks
    open SmartRecipes.UseCases.Environment
    open SmartRecipes.DataAccess
    
    let getEnvironment () = {
        IO = {
            Tokens = Tokens.dao
            Users = Users.dao
            Recipes = Recipes.dao
            ShoppingLists = ShoppingLists.dao
            Foodstuffs = Foodstuffs.dao
        }
        NowUtc = DateTime.UtcNow
    }
    
    let private setStatusCode (ctx: HttpContext) code =
        ctx.SetStatusCode code
            
    let private bindQueryString<'a> (ctx: HttpContext) =
        ctx.BindQueryString<'a>()
        
    let private bindModelAsync<'a> (ctx: HttpContext) =
        ctx.BindModelAsync<'a>()
        
    let private getHeader (ctx: HttpContext) name =
        ctx.GetRequestHeader(name)
        
    let private getResult env next ctx handler serialize parameters =
        let result = handler parameters |> ReaderT.execute env |> serialize
        let response =
            match result with 
            | Ok s -> setStatusCode ctx 200 |> (fun _ -> json s) 
            | Error e -> setStatusCode ctx 400 |> (fun _ -> json e)
        response next ctx
        
    let getHandler handler serialize next ctx = 
        task {
            return! bindQueryString<'parameters> ctx |> getResult (getEnvironment ()) next ctx handler serialize
        }
    
    let postHandler handler serialize next ctx = 
        task {
            let! parameters = bindModelAsync ctx 
            return! getResult (getEnvironment ()) next ctx handler serialize parameters
        }
            
    let authorizedGetHandler handler serialize next ctx = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! getHandler (handler accessToken) serialize next ctx
        }
    
    let authorizedPostHandler handler serialize next ctx = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! postHandler (handler accessToken) serialize next ctx
        }