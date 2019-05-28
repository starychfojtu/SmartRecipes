namespace SmartRecipes.Domain

module NonNegativeFloat =
    type NonNegativeFloat = private NonNegativeFloat of float
    
    let private nonNegativeFloat f = NonNegativeFloat f
    
    let create f =
        match f < 0.0 with 
        | true -> None
        | false -> Some <| NonNegativeFloat(f)
        
    let value (NonNegativeFloat f) = f
        
    let inline (-) a b =
        create (value a - value b)