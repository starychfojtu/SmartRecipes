module SmartRecipes.Recommendations.TextTfIdf

open System
open SmartRecipes.Recommendations
open Model
open Library

type DataSetStatistics = {
    NumberOfRecipes: float
    TermFrequencies: Map<string, float>
}

let termFrequency = 1.0

let tfIdf statistics term =
    let documentFrequency = Map.tryFind term statistics.TermFrequencies |> Option.defaultValue 1.0
    term, Math.Log10(statistics.NumberOfRecipes / documentFrequency) * termFrequency

type Vector = Map<string, float>

let vectorize statistics terms: Vector =
    List.map (tfIdf statistics) terms |> Map.ofList

let normalizeTerm (s: string) =
    s.ToLowerInvariant()

let ingredientToWords ingredient =
    ingredient.DisplayLine.Split([|' '|], StringSplitOptions.None)
    |> Array.toList
    |> List.map normalizeTerm
    |> List.where (fun w -> match ingredient.Amount.Unit with Some u -> u <> w | None -> true)
    |> List.where (Seq.forall (Char.IsDigit >> not))
    |> List.where (Seq.exists (Char.IsLetter))

let computeStatistics (recipes: Recipe list) =
    let recipeCount = List.length recipes
    let termFrequencies =
        recipes
        |> List.collect (fun r -> r.Ingredients)
        |> List.collect ingredientToWords
        |> List.groupBy id
        |> List.map (mapSecond (List.length >> float))
        |> Map.ofList

    { NumberOfRecipes = float recipeCount; TermFrequencies = termFrequencies }

let recommend recipes inputs =
    let statistics = computeStatistics recipes

    let inputVector = vectorize statistics (inputs |> List.map normalizeTerm)
    let recommendations =
        recipes
        |> Seq.map (fun r -> let vector = r.Ingredients |> List.collect ingredientToWords |> vectorize statistics in (r,  vector, cosineSimilarity inputVector vector))
        |> Seq.sortByDescending (fun (_, _, sim) -> sim)
        |> Seq.map (fun (r, _, _) -> r)

    recommendations