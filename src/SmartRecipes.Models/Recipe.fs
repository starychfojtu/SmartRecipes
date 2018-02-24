module Recipe

open System

type Ingredient = {
    food: Foodstuff;
    amount: int;
}

type Ingredients = seq<Ingredient>

type Recipe = {
    id: Guid;
    name: string;
    ingredients: Ingredients
}

type Recipes = seq<Recipe>