module Models.Foodstuff
    open Models.NonEmptyString
    open System
    open NonNegativeFloat
    open FSharpPlus
    open Infrastructure
    open Infrastructure.Validation
    
    type MetricUnit = 
        | Liter
        | Gram
        | Piece
    
    type Amount = {
        unit: MetricUnit;
        value: NonNegativeFloat
    }
    
    let createAmount unit value = {
        unit = unit
        value = value
    }
    
    type FoodstuffId = | FoodstuffId of Guid
        with member i.value = match i with FoodstuffId v -> v
    
    type Foodstuff = {
        id: FoodstuffId
        name: NonEmptyString;
        baseAmount: Amount
        amountStep: Amount
    }
    
    let private createFoodstuffId = Guid.NewGuid() |> FoodstuffId
    
    let private defaultBaseAmount = {
        unit = Gram
        value = mkNonNegativeFloat 100.0 |> forceSucces
    }
    
    let private defaultAmountStep = {
            unit = Gram
            value = mkNonNegativeFloat 10.0 |> forceSucces
        }
    
    let createFoodstuff name baseAmount amountStep = {
        id = createFoodstuffId
        name = name
        baseAmount = Option.defaultValue defaultBaseAmount baseAmount
        amountStep = Option.defaultValue defaultAmountStep amountStep
    }
    