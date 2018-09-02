module Api.Generic
    open DataAccess
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open DataAccess.Context
    open FSharpPlus.Data

    let authorizedGetHandler<'parameters, 'result, 'error> (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> string -> Reader<Context, Result<'result, 'error>>) = 
        task {
            let parameters = ctx.BindQueryString<'parameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let result = handler parameters accessToken |> Reader.execute (createDbContext())
            return! json result next ctx
        }
    
    let authorizedPostHandler<'parameters, 'result, 'error> (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> string -> Reader<Context, Result<'result, 'error>>) = 
        task {
            let! parameters = ctx.BindModelAsync<'parameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let result = handler parameters accessToken |> Reader.execute (createDbContext())
            return! json result next ctx
        }