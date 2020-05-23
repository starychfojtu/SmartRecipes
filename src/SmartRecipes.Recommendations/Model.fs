module SmartRecipes.Recommendations.Model

open System

type Foodstuff = {
    Id: Guid
    Name: string
}

type FoodstuffAmount = {
    Unit: string option
    Value: float option
    FoodstuffId: Guid
}

type Ingredient = {
    Amount: FoodstuffAmount
    Comment: string option
    DisplayLine: string
    RecipeId: Guid
}

type Recipe = {
    Id: Guid
    Name: string
    PersonCount: int
    ImageUrl: Uri
    Url: Uri
    Description: string
    Difficulty: string option
    CookingTime: string option
    Rating: int
    Tags: string list
    Ingredients: Ingredient list
    IngredientByFoodstuffId: Map<Guid, Ingredient>
}

type RecipeInfo = {
    Recipe: Recipe
    InputSimilarity: float
}
