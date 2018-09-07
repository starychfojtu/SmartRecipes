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
    open System
    
    type RecipeId = RecipeId of Guid

    type RecipeInfo = {
        id: RecipeId
        name: NonEmptyString
        creatorId: AccountId
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
    }
    
    let private createRecipeInfo name creatorId personCount imageUrl description = {
        id = RecipeId(Guid.NewGuid ())
        name = name
        creatorId = creatorId
        personCount = personCount
        imageUrl = imageUrl
        description = description
    }
    
    type Ingredient = {
        recipeId: RecipeId
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    type IngredientParameter = {
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let private createIngredient recipeId parameters = {
        recipeId = recipeId
        foodstuffId = parameters.foodstuffId
        amount = parameters.amount
    }
    
    let private createingredients recipeId = 
        NonEmptyList.map (createIngredient recipeId)
    
    type Recipe = {
        info: RecipeInfo
        ingredients: NonEmptyList<Ingredient>
    }
    
    let private createRecipeInternal info ingredients = { 
        info = info
        ingredients = ingredients
    }
    
    let createRecipe name creatorId personCount imageUrl description ingredientParameters =
        let info = createRecipeInfo name creatorId personCount imageUrl description
        let ingredients = createingredients info.id ingredientParameters
        createRecipeInternal info ingredients
        
        