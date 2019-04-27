namespace SmartRecipes.DataAccess

module Utils =
    open FSharpPlus.Data
    open SmartRecipes.Domain
    open System
    
    let forceSucces = function
        | Success s -> s
        | Failure f -> raise (InvalidOperationException("Invalid data in database."))
        
    
    let toNonEmptyStringModel s =
        NonEmptyString.mkNonEmptyString s |> forceSucces
        
    let toNaturalNumberModel n =
        NaturalNumber.create n |> forceSucces 