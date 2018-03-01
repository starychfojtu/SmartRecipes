namespace SmartRecipes.Models

open System

type MetricUnit = 
    | Liter
    | Gram
    | Piece

[<CLIMutable>]
type MetricUnitValue = {
    unit: MetricUnit;
    amount: int
}

type NutritionValues = {
    calories: int;
}

[<CLIMutable>]
type Foodstuff = {
    id: Guid
    name: string;
    nutritionValues: NutritionValues;
    baseAmount: MetricUnitValue
}