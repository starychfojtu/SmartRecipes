module SmartRecipes.Playground.Diversity

open SmartRecipes.Recommendations.Library

let tryMax list =
    try
        Some <| List.max list
    with _ ->
        None

let maximalMarginalRelevance lambda similarity results recipeInfo =
    let maxSimilarityToRankedRecipes =
        results
        |> List.map (fun (r: RecipeInfo) -> similarity recipeInfo.Recipe r.Recipe)
        |> tryMax
        |> Option.defaultValue 0.0

    lambda * recipeInfo.InputSimilarity - (1.0 - lambda) * maxSimilarityToRankedRecipes

let pickNextBestRecipe lambda similarity results candidates =
    candidates
    |> List.map (fun r -> (r, maximalMarginalRelevance lambda similarity results r))
    |> List.maxBy second
    |> first

let rec getRecommendations lambda similarity results candidates n =
    if List.isEmpty candidates
        then results
        else
            let nextBest = pickNextBestRecipe lambda similarity results candidates
            let newResults = nextBest::results
            if List.length newResults >= n
                then List.rev newResults
                else getRecommendations lambda similarity newResults (List.except [nextBest] candidates) n

let postProcess recipeInfos similarity n =
    let lambda = 0.5 // accuracy vs diversity ratio (the higher the better for accuracy)
    let recipeInfoCandidates =
        recipeInfos
        |> Seq.sortByDescending (fun i -> i.InputSimilarity)
        |> Seq.take (10 * n) // Filter only at least somehow relevant candidates.
        |> Seq.toList

    getRecommendations lambda similarity List.empty recipeInfoCandidates n
