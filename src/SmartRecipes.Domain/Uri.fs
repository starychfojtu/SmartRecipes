namespace SmartRecipes.Domain

module Uri =
    open FSharpPlus.Data
    open System

    let create uri =
        try
            Success (Uri(uri))
        with 
        | e -> Failure e.Message