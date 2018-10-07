module Api.Generic
    open DataAccess
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open FSharpPlus.Data
    open Domain.Token
    open FSharp.Control.Tasks
    
    let private setStatusCode (ctx: HttpContext) code =
        ctx.SetStatusCode code
            
    let private bindQueryString<'a> (ctx: HttpContext) =
        ctx.BindQueryString<'a>()
        
    let private bindModelAsync<'a> (ctx: HttpContext) =
        ctx.BindModelAsync<'a>()
        
    let private getHeader (ctx: HttpContext) name =
        ctx.GetRequestHeader(name)
        
    let private getResult dao next ctx handler serialize parameters =
        let result = handler parameters |> Reader.execute dao |> serialize
        let response =
            match result with 
            | Ok s -> setStatusCode ctx 200 |> (fun _ -> json s) 
            | Error e -> setStatusCode ctx 400 |> (fun _ -> json e)
        response next ctx
        
    let getHandler dao next ctx handler serialize = 
        task {
            return! bindQueryString<'parameters> ctx |> getResult dao next ctx handler serialize
        }
    
    let postHandler dao next ctx handler serialize = 
        task {
            let! parameters = bindModelAsync ctx 
            return! getResult dao next ctx handler serialize parameters
        }
            
    let authorizedGetHandler dao next ctx handler serialize = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! getHandler dao next ctx (handler accessToken) serialize
        }
    
    let authorizedPostHandler dao next ctx handler serialize = 
        task {
            let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
            return! postHandler dao next ctx (handler accessToken) serialize
        }