namespace SmartRecipes.Domain

module Foodstuff =
    open NonEmptyString
    open System
    open NonNegativeFloat
    open FSharpPlus
    open Infrastructure
    open Infrastructure.Validation
    
    type MetricUnit = 
        | MetricUnit of NonEmptyString
        with member u.Value = match u with MetricUnit s -> s
        
    module MetricUnits =
        let gram = NonEmptyString.create "gram" |> forceSucces |> MetricUnit
    
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
        name: NonEmptyString
        baseAmount: Amount
        amountStep: NonNegativeFloat
    }
    
    let private createFoodstuffId () = Guid.NewGuid() |> FoodstuffId
    
    let private defaultBaseAmount = {
        unit = MetricUnits.gram
        value = NonNegativeFloat.create 100.0 |> forceSucces
    }
    
    let private defaultAmountStep =
        NonNegativeFloat.create 10.0 |> forceSucces
    
    let createFoodstuff name baseAmount amountStep = {
        id = createFoodstuffId ()
        name = name
        baseAmount = Option.defaultValue defaultBaseAmount baseAmount
        amountStep = Option.defaultValue defaultAmountStep amountStep
    }
    
    // TODO: Implement proper solution.
    let convertToUnit foodstuff amount unit: Amount option =
        if unit = amount.unit
            then Some amount
            else None