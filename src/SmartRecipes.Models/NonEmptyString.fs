module Models.NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonEmptyString = NonEmptyString of string
        
    let private nonEmptyString s = NonEmptyString s
    
    type NonEpmtyStringError = | StringIsEmpty
    
    let private nonEmpty s =
        match String.IsNullOrEmpty s with 
        | true -> Failure StringIsEmpty
        | false -> Success s
    
    let mkNonEmptyString s =
        nonEmptyString
        <!> nonEmpty s