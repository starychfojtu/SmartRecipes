namespace SmartRecipes.Recommendations

module Recommendations =
    
    let sort input =
        Seq.sortByDescending (Distance.evaluate input)

