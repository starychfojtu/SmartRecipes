namespace SmartRecipes.Domain

module NaturalNumber =
    type NaturalNumber =
        private NaturalNumber of uint16
        with member s.Value = match s with NaturalNumber v -> v
    
    let create n = 
        if n > 0
            then Some <| NaturalNumber ((uint16)n)
            else None