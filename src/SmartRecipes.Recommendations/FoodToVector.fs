module SmartRecipes.Recommendations.FoodToVector

open System
open System.IO
open SmartRecipes.Recommendations.Model

let magnitude vector =
    vector
    |> Array.map (fun v -> Math.Pow(v, 2.0))
    |> Array.sum
    |> Math.Sqrt

let dotProduct v1 v2 =
    Array.zip v1 v2
    |> Array.map (fun (a, b) -> a * b)
    |> Array.sum

let cosineSimilarity v1 v2 =
    (dotProduct v1 v2) / ((magnitude v1) * (magnitude v2))

type WeightedVector = {
    Vector: float[]
    Weight: float
}

let mean (vectors: WeightedVector list) =
    let count = List.length vectors |> float
    let dimension = Array.length vectors.[0].Vector
    let result: float[] = Array.zeroCreate dimension
    for d in 0..dimension-1 do
        let sum = vectors |> List.map (fun v -> v.Vector.[d] * v.Weight) |> List.sum
        let mean = sum / count
        result.[d] <- mean

    result

type WeightedFoodstuff = {
    FoodstuffId: Guid
    Weight: float
}

let vectorize foodstuffVectors weight foodstuffAmounts  =
    foodstuffAmounts
    |> List.map (fun (a: FoodstuffAmount) -> { Vector = Map.find a.FoodstuffId foodstuffVectors; Weight = weight a })
    |> mean

let vectorizeRecipe foodstuffVectors weight r =
    let foodstuffAmounts = r.Ingredients |> List.map (fun i -> i.Amount)
    vectorize foodstuffVectors weight foodstuffAmounts

let recipeSimilarity foodstuffVectors weight r1 r2 =
    let toVector = vectorizeRecipe foodstuffVectors weight
    cosineSimilarity (toVector r1) (toVector r2)

module Data =
    let readLines (filePath : string) =
        seq {
            use sr = new StreamReader(filePath)
            sr.ReadLine() |> ignore // Skip header.
            sr.ReadLine() |> ignore // Skip space token.
            while not sr.EndOfStream do
                yield sr.ReadLine()
        }

    let parseLine (line: string) =
        let parts = line.Split([|' '|])
        let foodstuffId = Guid(parts.[0])
        let vector =
            parts
            |> Seq.skip 1
            |> Seq.filter (fun s -> s <> "")
            |> Seq.map float
            |> Seq.toArray

        (foodstuffId, vector)

    let loadFoodstuffVectors file =
         readLines file
         |> Seq.map parseLine
         |> Map.ofSeq

type Word2VecRecipeInfo = {
    Info: RecipeInfo
    Vector: float[]
}

let recommend foodstuffVectors weight inverseIndex foodstuffAmounts =
    let inputVector = vectorize foodstuffVectors weight foodstuffAmounts
    let relevantRecipes =
        foodstuffAmounts
        |> List.collect (fun a -> Map.find a.FoodstuffId inverseIndex)
        |> List.distinctBy (fun r -> r.Id)

    relevantRecipes
    |> List.map (fun (r: Recipe) ->
        let vector = vectorizeRecipe foodstuffVectors weight r
        let distance = cosineSimilarity vector inputVector
        { Info = { Recipe = r; InputSimilarity = distance }; Vector = vector }
    )
    |> List.sortByDescending (fun i -> i.Info.InputSimilarity)