module DataAccess.Utils
    open FSharpPlus.Data
    open Models
    open Models
    open System
    open Validation
    
    let forceSucces = function
        | Success s -> s
        | Failure f -> raise (InvalidOperationException("Invalid data in database."))
        
    
    let toNonEmptyStringModel s =
        NonEmptyString.mkNonEmptyString s |> forceSucces
        
    let toNaturalNumberModel n =
        NaturalNumber.mkNaturalNumber n |> forceSucces