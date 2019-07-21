namespace SmartRecipes.Recommendations
open SmartRecipes.Domain.Foodstuff

module Input =
    
    type Input = {
        FoodstuffIds: FoodstuffId list
    }

