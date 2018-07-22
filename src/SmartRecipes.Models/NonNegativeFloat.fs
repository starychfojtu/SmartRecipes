module Models.NonNegativeFloat
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonNegativeFloat = private NonNegativeFloat of float
        
    let private nonNegativeFloat f = NonNegativeFloat f
    
    type NonNegativeFloatError = | FloatIsNegative
    
    let private nonNegative f =
        match f < 0.0 with 
        | true -> Failure FloatIsNegative
        | false -> Success f
    
    let mkNonEmptyString f =
        nonNegativeFloat
        <!> nonNegative f