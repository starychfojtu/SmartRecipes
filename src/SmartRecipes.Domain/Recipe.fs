module Domain.Recipe
    open System.Xml.Linq
    open System
    open Account
    open FSharpPlus.Data
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data
    open Foodstuff
    open Domain.NaturalNumber
    open Domain.NonEmptyString
    open Domain.NonNegativeFloat
    open Domain.FoodstuffAmount
    open System
    
    type RecipeId = RecipeId of Guid
        with member i.value = match i with RecipeId v -> v

    type Recipe = {
        id: RecipeId
        name: NonEmptyString
        creatorId: AccountId
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<FoodstuffAmount>
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
        
        