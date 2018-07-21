module Models.Recipe
    open System
    open Account
    open Foodstuff
    
    type RecipeId = RecipeId of Guid
    
    type Recipe = {
        id: RecipeId;
        name: string;
        creatorId: AccountId;
    }
    
    type IngredientId = IngredientId of Guid
    
    type Ingredient = {
        id: IngredientId;
        recipeId: RecipeId;
        foodstuffId: FoodstuffId;
        amount: Amount;
    }