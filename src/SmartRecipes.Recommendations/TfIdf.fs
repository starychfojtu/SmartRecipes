module SmartRecipes.Recommendations.TfIdf

open System
open SmartRecipes.Recommendations
open Library
open SmartRecipes.Domain.Foodstuff
open SmartRecipes.Domain.Recipe
open FSharpPlus.Lens

type DataSetStatistics = {
    NumberOfRecipes: float
    FoodstuffFrequencies: Map<FoodstuffId, float>
    InverseIndex: Map<FoodstuffId, Recipe list>
}

// Term frequency ~ ingredient frequency in single recipe is 1 by default, but for chosen and most used unit, its amount is taken if present
let termFrequency = function
    | Some amount ->
        match amount.unit.Value.Value with
        | "cup" -> 2.0 * amount.value.Value
        | "cups" -> 2.0 * amount.value.Value
        | "pieces" -> 2.0 * amount.value.Value
        | "pound" -> 4.0 * amount.value.Value
        | "pounds" -> 4.0 * amount.value.Value
        | "gram" ->  amount.value.Value / 50.0
        | "grams" -> amount.value.Value / 50.0
        | _ -> 1.0
    | None -> 1.0

let tfIdf statistics (foodstuffAmount: FoodstuffAmount) =
    let documentFrequency = Map.find foodstuffAmount.FoodstuffId statistics.FoodstuffFrequencies
    let idf = Math.Log10(statistics.NumberOfRecipes / documentFrequency)
    let tf = termFrequency foodstuffAmount.Amount
    foodstuffAmount.FoodstuffId, idf * tf

let vectorize statistics foodstuffAmounts =
    Seq.map (tfIdf statistics) foodstuffAmounts |> Map.ofSeq

let getInverseIndex (recipes: Recipe list) =
    recipes
    |> Seq.collect (fun r -> view _ingredients r |> Seq.map (fun i -> (i.FoodstuffId, r)))
    |> Seq.groupBy (fun (foodstuffId, _) -> foodstuffId)
    |> Seq.map (fun (foodstuffId, values) ->
        let distinctRecipes =
            values
            |> Seq.map (fun (_, r) -> r)
            |> Seq.distinctBy (fun r -> r.Id)
            |> Seq.toList
        (foodstuffId, distinctRecipes))
    |> Map.ofSeq

let computeStatistics (recipes: Recipe list) =
    let recipeCount = List.length recipes |> float
    let inverseIndex = getInverseIndex recipes
    let foodstuffFrequencies =
        inverseIndex
        |> Map.map (fun k v -> Seq.length v |> float)

    {
        NumberOfRecipes = recipeCount
        FoodstuffFrequencies = foodstuffFrequencies
        InverseIndex = inverseIndex
    }

let vectorizeRecipe statistics recipe =
    recipe.Ingredients
    |> Seq.map (fun i -> { FoodstuffId = i.FoodstuffId; Amount = i.Amount })
    |> vectorize statistics

let recipeSimilarity statistics r1 r2 =
    cosineSimilarity (vectorizeRecipe statistics r1) (vectorizeRecipe statistics r2)

type TfIdfRecipeInfo = {
    Info: RecipeInfo
    Vector: Vector
}

let recommend statistics foodstuffAmounts =
    let inputVector = vectorize statistics foodstuffAmounts
    let relevantRecipes =
        foodstuffAmounts
        |> List.collect (fun a -> Map.find a.FoodstuffId statistics.InverseIndex)
        |> List.distinctBy (fun r -> r.Id)

    let recipesToRecommend =
        relevantRecipes
        |> Seq.map (fun r ->
            let vector = vectorizeRecipe statistics r
            { Info = { Recipe = r; InputSimilarity = cosineSimilarity vector inputVector }; Vector = vector })
        |> Seq.sortByDescending (fun r -> r.Info.InputSimilarity)

    recipesToRecommend