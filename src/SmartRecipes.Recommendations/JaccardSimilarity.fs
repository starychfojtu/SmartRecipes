module SmartRecipes.Recommendations.JaccardSimilarity

open FSharpPlus.Data
open SmartRecipes.Recommendations
open Library
open SmartRecipes.Domain.Recipe
open FSharpPlus.Lens
open Infrastructure

// This method is just a reference point for the others.
// Has no way of distinguishing importance of foodstuff (no amount factor, no relevance in the whole dataset factor).
// Moreover it prefers small size recipes.

let private jaccardDistance a b =
    let intersectionCount = Set.intersect a b |> Set.count |> float
    let unionCount = Set.union a b |> Set.count |> float
    intersectionCount / unionCount

let recommend recipes inputs =
    let inputSet = Set.ofList inputs
    let recommendations =
        recipes
        |> Seq.map (fun r -> (r, (r.Ingredients |> Seq.map (fun i -> i.FoodstuffId) |> Set.ofSeq |> jaccardDistance inputSet)))
        |> Seq.sortByDescending second
        |> Seq.map first

    recommendations