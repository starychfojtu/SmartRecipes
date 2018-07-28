module Models.Uri
    open FSharpPlus.Data
    open Models
    open System

    let mkUri uri =
        try
            Success (Uri(uri))
        with 
        | e -> Failure e.Message