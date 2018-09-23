module Domain.NonNegativeFloat
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data
    open System

    type NonNegativeFloat = private NonNegativeFloat of float
    
    let private nonNegativeFloat f = NonNegativeFloat f
    
    type NonNegativeFloatError = 
        | FloatIsNegative
    
    let create f =
        match f < 0.0 with 
        | true -> Failure FloatIsNegative
        | false -> Success(NonNegativeFloat(f))
        
    let value (NonNegativeFloat f) = f
        
    let inline (-) a b =
        create (value a - value b) |> Validation.toResult