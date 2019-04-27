namespace SmartRecipes.Api

module Generic =
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open FSharpPlus.Data
    open FSharp.Control.Tasks
    
    let private setStatusCode (ctx: HttpContext) code =
        ctx.SetStatusCode code
            
    let private bindQueryString<'a> (ctx: HttpContext) =
        ctx.BindQueryString<'a>()
        
    let private bindModelAsync<'a> (ctx: HttpContext) =
        ctx.BindModelAsync<'a>()
        
    let private getHeader (ctx: HttpContext) name =
        ctx.GetRequestHeader(name)
        
    let private getResult env next ctx handler serialize parameters =
        let result = handler parameters |> Reader.execute env |> serialize
        let response =
            match result with 
            | Ok s -> setStatusCode ctx 200 |> (fun _ -> json s) 
            | Error e -> setStatusCode ctx 400 |> (fun _ -> json e)
        response next ctx
        
    let getHandler env next ctx handler serialize = 
        task {
            return! bindQueryString<'parameters> ctx |> getResult env next ctx handler serialize
        }
    
    let postHandler env next ctx handler serialize = 
        task {
            let! parameters = bindModelAsync ctx 
            return! getResult env next ctx handler serialize parameters
        }
            
    let authorizedGetHandler env next ctx handler serialize = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! getHandler env next ctx (handler accessToken) serialize
        }
    
    let authorizedPostHandler env next ctx handler serialize = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! postHandler env next ctx (handler accessToken) serialize
        }