module Models.Foodstuff
    open Models.NonEmptyString
    open System
    open NonNegativeFloat
    open FSharpPlus
    
    type MetricUnit = 
        | Liter
        | Gram
        | Piece
    
    type Amount = {
        unit: MetricUnit;
        value: NonNegativeFloat
    }
    
    let private createAmount unit value = {
        unit = unit
        value = value
    }
    
    let mkAmount value unit = 
        createAmount unit
        <!> mkNonNegativeFloat value
    
    type FoodstuffId = FoodstuffId of Guid
    
    type Foodstuff = {
        id: FoodstuffId
        name: NonEmptyString;
        baseAmount: Amount
        amountStep: Amount
    }