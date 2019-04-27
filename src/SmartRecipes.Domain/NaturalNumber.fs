namespace SmartRecipes.Domain

module NaturalNumber =
    open FSharpPlus.Data
    open System
    
    type NaturalNumber = uint16
    
    type NaturalNumberError =
        | NumberIsNotNatural
    
    let create n = 
        if n > 0
            then Success (Convert.ToUInt16 n)
            else Failure NumberIsNotNatural