namespace SmartRecipes.Recommendations

open Library
open Calibration
open SmartRecipes.Domain.Recipe

module Recommendations =
    let calibratedWord2Vec statistics foodstuffVectors foodstuffAmounts =
        let weight = (TfIdf.tfIdf statistics) >> second
        let infos = FoodToVector.recommend foodstuffVectors weight statistics.InverseIndex foodstuffAmounts
        let recipes = infos |> List.map (fun i -> i.Info.Recipe)
        let recipeVectors = infos |> List.map (fun i -> (i.Info.Recipe.Id, i.Vector)) |> Map.ofList
        let similarity foodstuffAmountInfos (recipe: Recipe) =
            let weightsByFoodstuffAmount = foodstuffAmountInfos |> List.map (fun (i: FoodstuffAmountInfo) -> (i.Amount.FoodstuffId, i.Weight)) |> Map.ofList
            let queryVector = FoodToVector.vectorize foodstuffVectors (fun a -> Map.find a.FoodstuffId weightsByFoodstuffAmount) foodstuffAmounts
            let recipeVector = Map.find recipe.Id recipeVectors
            FoodToVector.cosineSimilarity queryVector recipeVector

        Calibration.calibrate similarity weight recipes foodstuffAmounts 10


