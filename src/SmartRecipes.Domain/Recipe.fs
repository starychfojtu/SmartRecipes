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
        foodstuffId: FoodstuffId
        amount: Amount
    }
    
    let createIngredient foodstuffId amount = {
        foodstuffId = foodstuffId
        amount = amount
    }
    
    type RecipeId = RecipeId of Guid
        with member i.value = match i with RecipeId v -> v

    type Recipe = {
        id: RecipeId
        name: NonEmptyString
        creatorId: AccountId
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<Ingredient>
    }
    
    let createRecipe name creatorId personCount imageUrl description ingredients = {
        id = RecipeId(Guid.NewGuid ())
        name = name
        creatorId = creatorId
        personCount = personCount
        imageUrl = imageUrl
        description = description
        ingredients = ingredients
    }
        
        