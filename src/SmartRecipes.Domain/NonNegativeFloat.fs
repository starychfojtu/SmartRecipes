module Domain.NonNegativeFloat
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data
    open System

    type NonNegativeFloat = private NonNegativeFloat of float
        with member f.value = match f with NonNegativeFloat v -> v
            
        
    let private nonNegativeFloat f = NonNegativeFloat f
    
    type NonNegativeFloatError = | FloatIsNegative
    
    let private nonNegative f =
        match f < 0.0 with 
        | true -> Failure FloatIsNegative
        | false -> Success f
    
    let mkNonNegativeFloat f =
        nonNegativeFloat
        <!> nonNegative f
        
    let inline (-) (a : NonNegativeFloat) (b : NonNegativeFloat) =
        mkNonNegativeFloat (a.value - b.value) |> Validation.toResult