module Api.Generic
    open DataAccess
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open DataAccess.Context
    open FSharpPlus.Data
    open Models.Token

    let getHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> Reader<'dao, Result<'result, 'error>>) = 
        task {
            let parameters = ctx.BindQueryString<'parameters>()
            let result = handler parameters |> Reader.execute dao
            return! json result next ctx
        }
    
    let postHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> Reader<'dao, Result<'result, 'error>>) = 
        task {
            let! parameters = ctx.BindModelAsync<'parameters>()
            let result = handler parameters |> Reader.execute dao
            return! json result next ctx
        }
            
    let authorizedGetHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: string -> 'parameters -> Reader<'dao, Result<'result, 'error>>) = 
        task {
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            return! getHandler dao next ctx (handler accessToken)
        }
    
    let authorizedPostHandler dao (next : HttpFunc) (ctx : HttpContext) (handler: string -> 'parameters -> Reader<'dao, Result<'result, 'error>>) = 
        task {
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            return! postHandler dao next ctx (handler accessToken)
        }