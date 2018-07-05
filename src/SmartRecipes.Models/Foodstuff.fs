module Models.Foodstuff
    open System
    open System
    
    type MetricUnit = 
        | Liter
        | Gram
        | Piece
    
    type Amount = {
        unit: MetricUnit;
        amount: float
    }
    
    type FoodstuffId = FoodstuffId of Guid
    
    type Foodstuff = {
        id: FoodstuffId
        name: string;
        baseAmount: Amount
        amountStep: Amount
    }