[<RequireQualifiedAccess>]
module Business.Recipes
    open Models
    open Models.Foodstuff
    open Models.Recipe
    open System
    open NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Validation
    open Infrastructure.NonEmptyList
    open Models.Account
    open Models.Recipe
    open NaturalNumber
    open Models.Uri
    
    type IngredientParameter = {
        foodstuff: Foodstuff
        amount: float
    }
        
    type CreateParameters = {
        name: string
        personCount: int
        description: string
        imageUrl: string
        ingredients: seq<IngredientParameter>
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | AmountOfFoodstuffMustBePositive
        | MustContaintAtLeastOneIngredient
        
    let private createIngredient recipeId parameter =
        mkIngredient recipeId parameter.foodstuff.id 
        <!> mkAmount parameter.amount parameter.foodstuff.baseAmount.unit
        
    let private createIngredients parameters (recipeInfo: RecipeInfo) =
        parameters
        |> Seq.traverse (fun i -> 
            createIngredient recipeInfo.id i
            |> mapFailure (fun _ -> [AmountOfFoodstuffMustBePositive]) 
            |> Validation.toResult)
        |> Validation.ofResult
    
    let create (accountId: AccountId) parameters =
        let recipeInfo = 
            mkRecipeInfo
            <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
            <*> Success accountId
            <*> (mkNaturalNumber parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
            <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
            <*> Success parameters.description
            
        let ingredients = 
            Validation.bind (createIngredients parameters.ingredients) recipeInfo
            |> Validation.bind (fun s -> mkNonEmptyList s |> mapFailure (fun _ -> [MustContaintAtLeastOneIngredient]))
        
        mkRecipe 
        <!> recipeInfo 
        <*> ingredients
        
        
    