module Models.Foodstuff
    open Models.NonEmptyString
    open System
    open NonNegativeFloat
    
    type MetricUnit = 
        | Liter
        | Gram
        | Piece
    
    type Amount = {
        unit: MetricUnit;
        value: NonNegativeFloat
    }
    
    type FoodstuffId = FoodstuffId of Guid
    
    type Foodstuff = {
        id: FoodstuffId
        name: NonEmptyString;
        baseAmount: Amount
        amountStep: Amount
    }