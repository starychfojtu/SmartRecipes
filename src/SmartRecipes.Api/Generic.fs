module Api.Generic
    open DataAccess
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open FSharpPlus.Data
    open Domain.Token
    open FSharp.Control.Tasks
    
    let getHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> Reader<'dao, Result<'result, 'error>>) serialize = 
        task {
            let parameters = ctx.BindQueryString<'parameters>()
            let result = handler parameters |> Reader.execute dao |> serialize
            let response = match result with | Ok s -> json s | Error e -> json e
            return! response next ctx
        }
    
    let postHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> Reader<'dao, Result<'result, 'error>>) serialize = 
        task {
            let! parameters = ctx.BindModelAsync<'parameters>()
            let result = handler parameters |> Reader.execute dao |> serialize
            let response = match result with | Ok s -> json s | Error e -> json e
            return! response next ctx
        }
            
    let authorizedGetHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: string -> 'parameters -> Reader<'dao, Result<'result, 'error>>) serialize = 
        task {
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            return! getHandler dao next ctx (handler accessToken) serialize
        }
    
    let authorizedPostHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: string -> 'parameters -> Reader<'dao, Result<'result, 'error>>) serialize = 
        task {
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            return! postHandler dao next ctx (handler accessToken) serialize
        }