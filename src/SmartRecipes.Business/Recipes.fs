[<RequireQualifiedAccess>]
module Business.Recipes
    open Models
    open Models.Foodstuff
    open Models.Recipe
    open System
    open NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Validation
    open Infrastructure.NonEmptyList
    open Models.Account
    open Models.Recipe
    open NaturalNumber
    open Models.Uri
    
//    type IngredientParameter = {
//        foodstuff: Foodstuff
//        amount: float
//    }
//        
//    type RecipeInfoParameters = {
//        name: string
//        personCount: int
//        description: string
//        imageUrl: string
//    }
//    
//    type CreateError =
//        | NameCannotBeEmpty
//        | PersonCountMustBePositive
//        | InvalidImageUrl of string
//        | AmountOfFoodstuffMustBePositive
//        | MustContaintAtLeastOneIngredient
//        
//    let private createIngredient recipeId parameter =
//        mkIngredient recipeId parameter.foodstuff.id 
//        <!> mkAmount parameter.amount parameter.foodstuff.baseAmount.unit
//        
//    // TODO: Implement traverse on Validation
//    let private createIngredients parameters recipeInfo =
//        parameters
//        |> Seq.traverse (fun i -> 
//            createIngredient recipeInfo.id i
//            |> mapFailure (fun _ -> [AmountOfFoodstuffMustBePositive]) 
//            |> Validation.toResult)
//        |> Validation.ofResult
//        
//    let private createRecipeInfo accountId parameters =
//        mkRecipeInfo
//        <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
//        <*> Success accountId
//        <*> (mkNaturalNumber parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
//        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
//        <*> Success parameters.description
//    
//    let create accountId infoParameters ingredientParameters =
//        let recipeInfo = 
//            createRecipeInfo accountId infoParameters
//        
//        let ingredients = 
//            Validation.bind (createIngredients ingredientParameters) recipeInfo
//            |> Validation.bind (fun s -> mkNonEmptyList s |> mapFailure (fun _ -> [MustContaintAtLeastOneIngredient]))
//        
//        mkRecipe 
//        <!> recipeInfo 
//        <*> ingredients
//        |> Validation.toResult
        
        
    