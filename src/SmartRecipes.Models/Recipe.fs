namespace SmartRecipes.Models

type Ingredient = {
    food: Foodstuff;
    amount: int;
}

type Recipe = {
    name: string;
    ingredients: seq<Ingredient>
}