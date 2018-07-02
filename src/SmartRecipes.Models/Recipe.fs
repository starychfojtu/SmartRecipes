namespace SmartRecipes.Models

open System

type RecipeId = Guid

[<CLIMutable>]
type Recipe = {
    id: RecipeId;
    name: string;
    creatorId: AccountId;
}

type IngredientId = Guid

[<CLIMutable>]
type Ingredient = {
    id: IngredientId;
    recipeId: RecipeId;
    foodstuffId: FoodstuffId;
    amount: Amount;
}