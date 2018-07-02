namespace SmartRecipes.Models

open System

type MetricUnit = 
    | Liter
    | Gram
    | Piece

[<CLIMutable>]
type Amount = {
    unit: MetricUnit;
    amount: float
}

type FoodstuffId = Guid

[<CLIMutable>]
type Foodstuff = {
    id: FoodstuffId
    name: string;
    baseAmount: Amount
    amountStep: Amount
}