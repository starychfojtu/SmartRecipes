module Models.Recipe
    open System
    open Account
    open FSharpPlus.Data
    open FSharpPlus
    open Foodstuff
    open Models.NonEmptyString
    
    type RecipeId = RecipeId of Guid
    
    type Recipe = {
        id: RecipeId;
        name: NonEmptyString;
        creatorId: AccountId;
    }
    
    type IngredientId = IngredientId of Guid
    
    type Ingredient = {
        id: IngredientId;
        recipeId: RecipeId;
        foodstuffId: FoodstuffId;
        amount: Amount;
    }