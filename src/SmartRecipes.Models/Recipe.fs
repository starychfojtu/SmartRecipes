namespace SmartRecipes.Models

open System

type Ingredient = {
    food: Foodstuff;
    amount: int;
}

type Ingredients = seq<Ingredient>

type RecipeStep = {
    number: int;
    ingredients: Ingredients;
    text: string;
}

type RecipeSteps = seq<RecipeStep>

[<CLIMutable>]
type Recipe = {
    id: Guid;
    name: string;
    creator: Account;
    ingredients: Ingredients;
    steps: RecipeSteps;
}

type Recipes = seq<Recipe>