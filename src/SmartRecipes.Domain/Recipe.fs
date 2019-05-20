namespace SmartRecipes.Domain

module Recipe =
    open Account
    open System
    open FSharpPlus.Data
    open Foodstuff
    open NaturalNumber
    open NonEmptyString
    open NonNegativeFloat
    
    type Ingredient = {
        FoodstuffId: FoodstuffId
        Amount: Amount option // Sometimes it is not specified how much of it should be used, eg. 'Olive oil'.
    }
    
    let createIngredient foodstuffId amount = {
        FoodstuffId = foodstuffId
        Amount = amount
    }
    
    type RecipeId = RecipeId of Guid
        with member i.value = match i with RecipeId v -> v

    type Recipe = {
        Id: RecipeId
        Name: NonEmptyString
        CreatorId: AccountId
        PersonCount: NaturalNumber
        ImageUrl: Uri
        Description: NonEmptyString option
        Ingredients: NonEmptyList<Ingredient>
    }
    
    let createRecipe name creatorId personCount imageUrl description ingredients = {
        Id = RecipeId(Guid.NewGuid ())
        Name = name
        CreatorId = creatorId
        PersonCount = personCount
        ImageUrl = imageUrl
        Description = description
        Ingredients = ingredients
    }
        
        