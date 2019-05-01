namespace SmartRecipes.Domain

module NonEmptyString =
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonEmptyString =
        private NonEmptyString of string
        with member s.Value = match s with NonEmptyString v -> v
    
    type NonEpmtyStringError =
        | StringIsEmpty
    
    let private nonEmpty s =
        match String.IsNullOrEmpty s with 
        | true -> Failure StringIsEmpty
        | false -> Success s
    
    let create s =
        NonEmptyString
        <!> nonEmpty s