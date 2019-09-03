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
        let gram = NonEmptyString.create "gram" |> Option.get |> MetricUnit
    
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
        
    module FoodstuffId = 
        let inline _value f (FoodstuffId id) = map FoodstuffId (f id)
    
    type Foodstuff = {
        id: FoodstuffId
        name: NonEmptyString
        // Default amount for shopping list. The average amount people put in shopping basket.
        // E.g. for milk it is 1 liter.
        baseAmount: Amount
        // The average amount of adjusting the default one.
        // E.g. default amount for chicken breasts is 0.8kg, because people usually buy 4 brests,
        // but they adjust it by 1, so amount step would be 0.2kg
        amountStep: NonNegativeFloat
    }
    
    let private createFoodstuffId () = Guid.NewGuid() |> FoodstuffId
    
    let private defaultBaseAmount = {
        unit = MetricUnits.gram
        value = NonNegativeFloat.create 100.0 |> Option.get
    }
    
    let private defaultAmountStep =
        NonNegativeFloat.create 10.0 |> Option.get
    
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