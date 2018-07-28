module Models.NonNegativeFloat
    open FSharpPlus
    open FSharpPlus.Data
    open System

    type NonNegativeFloat = NonNegativeFloat of float
        
    let private nonNegativeFloat f = NonNegativeFloat f
    
    type NonNegativeFloatError = | FloatIsNegative
    
    let private nonNegative f =
        match f < 0.0 with 
        | true -> Failure FloatIsNegative
        | false -> Success f
    
    let mkNonNegativeFloat f =
        nonNegativeFloat
        <!> nonNegative f