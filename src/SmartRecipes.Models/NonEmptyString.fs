module Models.NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonEmptyString = private NonEmptyString of string
        with member s.value = match s with NonEmptyString v -> v
        
    let private nonEmptyString s = NonEmptyString s
    
    type NonEpmtyStringError = | StringIsEmpty
    
    let private nonEmpty s =
        match String.IsNullOrEmpty s with 
        | true -> Failure StringIsEmpty
        | false -> Success s
    
    let mkNonEmptyString s =
        nonEmptyString
        <!> nonEmpty s
        
    let safeMkNonEmptyString s = 
        if isNull s then Failure NonEpmtyStringError.StringIsEmpty else mkNonEmptyString s