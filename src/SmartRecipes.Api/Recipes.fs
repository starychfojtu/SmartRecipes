namespace SmartRecipes.Api

module Recipes =
    open Dto
    open SmartRecipes.Domain
    open Generic
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.DataAccess
    open System
    open Infrastructure
    open SmartRecipes.UseCases
    open FSharpPlus.Data
    open SmartRecipes.UseCases.Recipes
    open Infrastructure.Validation
    open Uri
    open FSharpPlus
    open Foodstuffs
    open Infrastracture
    open Infrastructure.NonEmptyList
            
    // Get my recipes
    
    type GetMyRecipesResponse = {
        Recipes: RecipeDto seq
    }
    
    let private serializeGetMyRecipes = 
        Result.bimap (fun rs -> { Recipes = Seq.map serializeRecipe rs }) (function GetMyRecipesError.Unauthorized -> "Unauthorized.")
    
    let getMyRecipes accessToken parameters = 
        Recipes.getMyRecipes accessToken
    
    let getMyRecipesHandler<'a> =
        authorizedGetHandler getMyRecipes serializeGetMyRecipes
            
    // Create
    
    [<CLIMutable>]
    type IngredientParameter = {
        foodstuffId: Guid
        amount: AmountParameters
    }

    [<CLIMutable>]
    type CreateParameters = {
        name: string
        personCount: int
        imageUrl: string
        description: string
        ingredients: seq<IngredientParameter>
    }
    
    type CreateResponse = {
        Recipe: RecipeDto
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | AmountError of ParseAmountError
        | MustContaintAtLeastOneIngredient
        | DescriptionIsProvidedButEmpty
        | BusinessError of Recipes.CreateError
        
    let private createParameters name personCount imageUrl description ingredients: RecipeParameters = {
        name = name
        personCount = personCount
        imageUrl = imageUrl
        description = description
        ingredients = ingredients
    }
    
    let private createIngredientParameter foodstuffId amount: Recipes.IngredientParameters = {
        foodstuffId = foodstuffId
        amount = amount
    }
    
    let private parseIngredientAmount =
        parseAmount >> Validation.mapFailure (List.map AmountError)
        
    let private mkIngredientParameter parameter =
        createIngredientParameter parameter.foodstuffId
        <!> parseIngredientAmount parameter.amount
        
    let private toNonEmpty ingredients = 
        NonEmptyList.mkNonEmptyList ingredients 
        |> Validation.mapFailure (function SequenceIsEmpty -> [MustContaintAtLeastOneIngredient])
           
    let private mkIngredientParameters parameters =
        Seq.map mkIngredientParameter parameters
        |> Validation.traverseSeq
        |> Validation.bind toNonEmpty

    let private mkDescription d =
        if isNull d
            then Success None
            else NonEmptyString.create d |> Validation.bimap (fun _ -> [DescriptionIsProvidedButEmpty]) Some 
        
    let private mkParameters (parameters: CreateParameters) =
        createParameters
        <!> (NonEmptyString.create parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
        <*> (NaturalNumber.create parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> mkDescription parameters.description
        <*> (mkIngredientParameters parameters.ingredients)
        
    let private createRecipe accessToken = 
        Recipes.create accessToken >> (ReaderT.mapError (fun e -> [BusinessError e]))
        
    let private serializeCreateIngredientError = function
        | DuplicateFoodstuffIngredient -> "Multiple ingredients with common foodstuff found."
        | FoodstuffNotFound -> "Foodstuff not found."
        
    let private serializeCreateError = function 
        | NameCannotBeEmpty -> ["Name cannot be empty."]
        | PersonCountMustBePositive -> ["Person count must be positive."]
        | InvalidImageUrl s -> [s]
        | AmountError e ->
            match e with
            | UnknownUnit -> ["Unknown unit."]
            | ValueCannotBeNegative -> ["Amount of ingredient must be positive."]
        | MustContaintAtLeastOneIngredient -> ["Must containt at least one ingredient."]
        | DescriptionIsProvidedButEmpty -> ["Description is provided but empty."]
        | BusinessError e -> 
            match e with
            | Recipes.CreateError.Unauthorized -> ["Unauthorized."]
            | Recipes.CreateError.InvalidIngredients es -> List.map serializeCreateIngredientError es
        
    let private serializeCreate =
        Result.bimap (fun r -> { Recipe = serializeRecipe r }) (Seq.collect serializeCreateError)

    let create accessToken parameters = 
        mkParameters parameters |> Validation.toResult |> ReaderT.id
        >>= createRecipe accessToken

    let createHandler<'a> =
        authorizedPostHandler create serializeCreate
