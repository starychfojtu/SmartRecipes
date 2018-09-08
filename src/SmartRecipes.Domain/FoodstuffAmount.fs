module Domain.FoodstuffAmount
    open Domain.Foodstuff
    open Domain.NonNegativeFloat
    
    type FoodstuffAmount = {
        foodstuff: Foodstuff
        amount: NonNegativeFloat
    }
    
    let createFoodstuffAmount foodstuff amount = {
        foodstuff = foodstuff
        amount = amount
    }