namespace SmartRecipes.Api.HttpHandlers

open System
open Giraffe

[<RequireQualifiedAccess>]
module Generic =

    let detail query id =
        match Guid.TryParse(id) with
            | (true, guid) -> 
                match query guid with
                    | Some entity -> json entity 
                    | None -> setStatusCode 404
            | (false, _) -> setStatusCode 400
