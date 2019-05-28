namespace SmartRecipes.Domain

module NonEmptyString =
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonEmptyString =
        private NonEmptyString of string
        with member s.Value = match s with NonEmptyString v -> v
    
    let create s =
        match String.IsNullOrEmpty s with 
        | true -> None
        | false -> Some <| NonEmptyString s