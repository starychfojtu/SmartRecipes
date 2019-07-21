namespace SmartRecipes.Recommendations

module Recommendation =
    
    let sortByRecommendation input =
        Seq.sortByDescending (Distance.evaluate input)

