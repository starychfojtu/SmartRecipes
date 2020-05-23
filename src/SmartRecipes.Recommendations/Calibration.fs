module SmartRecipes.Recommendations.Calibration

open SmartRecipes.Recommendations.Library
open SmartRecipes.Domain.Recipe

type FoodstuffAmountInfo = {
    Amount: FoodstuffAmount
    InitialWeight: float
    WeightModifier: float
} with
    member this.Weight = this.InitialWeight / this.WeightModifier

let relevance (recipe: Recipe) (foodstuffAmount: FoodstuffAmount) =
    let ingredient = recipe.Ingredients |> Seq.filter (fun i -> i.FoodstuffId = foodstuffAmount.FoodstuffId) |> Seq.tryHead
    ingredient
    |> Option.map (fun i -> TfIdf.termFrequency i.Amount)
    |> Option.defaultValue 0.0

let adjustInfo bestRecipe info =
    {
        Amount = info.Amount
        InitialWeight = info.InitialWeight
        WeightModifier = info.WeightModifier + (relevance bestRecipe info.Amount)
    }

let calibrateStep similarity (recipes, foodstuffInfos, recommendations: Recipe list) =
    let bestRecipe = recipes |> List.maxBy (similarity foodstuffInfos)
    let newFoodstuffInfos = foodstuffInfos |> List.map (adjustInfo bestRecipe)
    let newRecipeInfos = List.except [bestRecipe] recipes
    (newRecipeInfos, newFoodstuffInfos, bestRecipe::recommendations)

let toInitialInfo weight amount =
    {
        Amount = amount
        InitialWeight = weight amount
        WeightModifier = 1.0
    }

let calibrate similarity weight recipes foodstuffAmounts n =
    let initialFoodstuffInfos = foodstuffAmounts |> List.map (toInitialInfo weight)
    let initialState = (recipes, initialFoodstuffInfos, [])
    let (_, _, recommendations) =
        Seq.replicate n ()
        |> Seq.fold (fun state _ -> calibrateStep similarity state) initialState

    recommendations
    |> List.rev
