namespace SmartRecipes.Models

open System

[<CLIMutable>]
type Recipe = {
    id: Guid;
    name: string;
    creator: Account;
}

[<CLIMutable>]
type Ingredient = {
    id: Guid;
    food: Foodstuff;
    amount: int;
    recipe: Recipe;
}

[<CLIMutable>]
type RecipeStep = {
    id: Guid;
    number: int;
    text: string;
    recipe: Recipe;
}