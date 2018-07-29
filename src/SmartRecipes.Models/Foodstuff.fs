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
    
    type FoodstuffId = | FoodstuffId of Guid
        with member i.value = match i with FoodstuffId v -> v
    
    type Foodstuff = {
        id: FoodstuffId
        name: NonEmptyString;
        baseAmount: Amount
        amountStep: Amount
    }