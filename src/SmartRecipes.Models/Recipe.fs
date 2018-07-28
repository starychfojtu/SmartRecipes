module Models.Recipe
    open System
    open Account
    open FSharpPlus.Data
    open FSharpPlus
    open FSharpPlus.Data
    open Foodstuff
    open Models.NaturalNumber
    open Models.NonEmptyString
    open System
    
    type RecipeId = RecipeId of Guid
    
    let private mkRecipeId () = RecipeId(Guid.NewGuid ())
    
    type RecipeInfo = {
        id: RecipeId
        name: NonEmptyString
        creatorId: AccountId
        personCount: NaturalNumber
        imageUrl: Uri
        description: string
    }
    
    let mkRecipeInfo name creatorId personCount imageUrl description = {
        id = mkRecipeId ()
        name = name
        creatorId = creatorId
        personCount = personCount
        imageUrl = imageUrl
        description = description
    }
    
    type Ingredient = {
        recipeId: RecipeId
        foodstuffId: FoodstuffId
        amount: Amount
    }
    
    let mkIngredient recipeId foodstuffId amount = {
        recipeId = recipeId
        foodstuffId = foodstuffId
        amount = amount
    }
    
    type Recipe = {
        info: RecipeInfo
        ingredients: NonEmptyList<Ingredient>
    } 
    
    let private ingredientsBelongToRecipe recipeId ingredients =
        NonEmptyList.toSeq ingredients
        |> Seq.forall (fun i -> i.recipeId = recipeId)
    
    let mkRecipe (info: RecipeInfo) ingredients = 
        match ingredientsBelongToRecipe info.id ingredients with 
        | false -> Error ()
        | true -> Ok { info = info; ingredients = ingredients }
        