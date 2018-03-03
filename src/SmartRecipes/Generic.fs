namespace SmartRecipes.Api

open System
open Giraffe

module Generic =
    open Microsoft.AspNetCore.Http

    type Result<'entity, 'error> =
        | Success of 'entity
        | Failure of 'error

    let bind (f: 'a -> Result<'b, 'e>) (g: 'b -> Result<'c, 'e>) (a: 'a): Result<'c, 'e> = 
        match f a with
            | Success a -> a |> g
            | Failure fail -> Failure fail

    let (>>=) = bind

    let bindParameters<'parameters, 'error> (ctx: HttpContext): Result<'parameters, 'error> =
        ctx.BindJsonAsync<'parameters>() 
        |> Async.AwaitTask 
        |> Async.RunSynchronously 
        |> Success

    let showResponse = function
        | Success entity -> json entity
        | Failure error -> json error

    let detail query id =
        match Guid.TryParse(id) with
            | (true, guid) -> 
                match query guid with
                    | Some entity -> json entity 
                    | None -> setStatusCode 404
            | (false, _) -> setStatusCode 400
