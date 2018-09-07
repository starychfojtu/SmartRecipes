module Domain.Uri
    open FSharpPlus.Data
    open Domain
    open System

    let mkUri uri =
        try
            Success (Uri(uri))
        with 
        | e -> Failure e.Message