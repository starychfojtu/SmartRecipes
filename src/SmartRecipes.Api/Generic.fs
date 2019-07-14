namespace SmartRecipes.Api

open System.Threading.Tasks
open FSharp.Json
open Infrastructure
open SmartRecipes.Domain

module Parse =
    let option v =
        Option.map v >> Validation.traverseOption
        
    let seqOf v =
        Seq.map v >> Validation.traverseSeq
    
    let nonEmptyString error =
        NonEmptyString.create >> Validation.ofOption error
        
    let nonEmptyStringOption error =
        nonEmptyString error |> option
        
    let uriOption error =
        option (Uri.create >> Validation.mapFailure error)
        
    let naturalNumber error =
        NaturalNumber.create >> Validation.ofOption error
        
    let nonNegativeFloat error =
        NonNegativeFloat.create >> Validation.ofOption error
            
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
    
    let private deserialize<'a> json =
        try
            Ok <| Json.deserialize<'a> json
        with
            | ex -> Error ex.Message
            
    let private serializeToJson obj =
        Json.serializeEx (JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)) obj
    
    let private setStatusCode (ctx: HttpContext) code =
        ctx.SetStatusCode code
            
    let private bindQueryString<'a> (ctx: HttpContext) =
        try 
            ctx.BindQueryString<'a>()
        with ex -> failwith ex.Message
        
    let private bindModelAsync<'a> (ctx: HttpContext): Task<Result<'a, string>> = task {
        let! body = ctx.ReadBodyFromRequestAsync ()
        return deserialize body
    }
        
    let private getHeader (ctx: HttpContext) name =
        ctx.GetRequestHeader(name)
        
    let private getResult env next ctx handler serialize parameters =
        let result = handler parameters |> ReaderT.execute env |> serialize
        let response =
            match result with 
            | Ok s -> setStatusCode ctx 200 |> (fun _ -> text <| serializeToJson s) 
            | Error e -> setStatusCode ctx 400 |> (fun _ -> text <| serializeToJson e)
        response next ctx
        
    let getHandler handler serialize next ctx = 
        bindQueryString<'parameters> ctx |> getResult (getEnvironment ()) next ctx handler serialize
    
    let postHandler handler serialize next ctx = 
        task {
            let! parameters = bindModelAsync ctx
            return!
                match parameters with
                | Ok p -> getResult (getEnvironment ()) next ctx handler serialize p
                | Error e -> getResult () next ctx (fun _ -> Error e |> ReaderT.id) (fun e -> e) ()
        }
            
    let authorizedGetHandler handler serialize next ctx = 
        let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
        getHandler (handler accessToken) serialize next ctx
    
    let authorizedPostHandler handler serialize next ctx =
        let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
        postHandler (handler accessToken) serialize next ctx