module Api.Recipes
    open Api
    open Dto
    open Domain
    open Generic
    open System.Net.Http
    open NonEmptyString
    open DataAccess
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open UseCases
    open DataAccess.Foodstuffs
    open DataAccess.Recipes
    open DataAccess.Tokens
    open Domain.Foodstuff
    open Domain.Recipe
    open FSharpPlus.Data
    open Infrastructure
    open UseCases.Recipes
    open Infrastructure.Validation
    open NaturalNumber
    open Uri
    open FSharpPlus
    open Infrastructure
    open Infrastructure.NonEmptyList
    open Infrastructure.Reader
    open UseCases.Recipes
            
    // Get my recipes
    
    let private getMyRecipesDao: GetMyRecipesDao = {
        tokens = Tokens.dao
        recipes = Recipes.dao
    }
    
    let private serializeGetMyRecipes = 
        Result.map (Seq.map serializeRecipe) >> Result.mapError (function Recipes.GetMyRecipesError.Unauthorized -> "Unauthorized.")
    
    let getMyRecipes accessToken parameters = 
        Recipes.getMyRecipes accessToken
    
    let getMyRecipesHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler getMyRecipesDao next ctx getMyRecipes serializeGetMyRecipes
            
    // Create
    
    [<CLIMutable>]
    type IngredientParameter = {
        foodstuffId: Guid
        amount: float
    }

    [<CLIMutable>]
    type CreateParameters = {
        name: string
        personCount: int
        imageUrl: string
        description: string
        ingredients: seq<IngredientParameter>
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | AmountOfIngredientMustBePositive
        | MustContaintAtLeastOneIngredient
        | DescriptionIsProvidedButEmpty
        | BusinessError of Recipes.CreateError
        
    let private createDao = {
        foodstuffs = Foodstuffs.dao
        recipes = Recipes.dao
        tokens = Tokens.dao
    }
        
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
        
    let private mkIngredientParameter parameter =
        createIngredientParameter parameter.foodstuffId
        <!> (NonNegativeFloat.create parameter.amount |> mapFailure (function FloatIsNegative -> [AmountOfIngredientMustBePositive]))
        
    let private toNonEmpty ingredients = 
        NonEmptyList.mkNonEmptyList ingredients 
        |> Validation.mapFailure (function SequenceIsEmpty -> [MustContaintAtLeastOneIngredient])
           
    let private mkIngredientParameters parameters =
        Seq.map mkIngredientParameter parameters 
        |> Validation.traverse
        |> Validation.bind toNonEmpty

    let private mkDescription d =
        if isNull d
            then Success None
            else mkNonEmptyString d |> mapFailure (fun _ -> [DescriptionIsProvidedButEmpty]) |> Validation.map Some 
        
    let private mkParameters (parameters: CreateParameters) =
        createParameters
        <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
        <*> (NaturalNumber.create parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> mkDescription parameters.description
        <*> (mkIngredientParameters parameters.ingredients)
        
    let private createRecipe accessToken = 
        Recipes.create accessToken >> (Reader.map (Result.mapError (fun e -> [BusinessError e])))
        
    let private serializeCreateIngredientError = function
        | DuplicateFoodstuffIngredient -> "Multiple ingredients with common foodstuff found."
        | FoodstuffNotFound -> "Foodstuff not found."
        
    let private serializeCreateError = function 
        | NameCannotBeEmpty -> ["Name cannot be empty."]
        | PersonCountMustBePositive -> ["Person count must be positive."]
        | InvalidImageUrl s -> [s]
        | AmountOfIngredientMustBePositive -> ["Amount of ingredient must be positive."]
        | MustContaintAtLeastOneIngredient -> ["Must containt at least one ingredient."]
        | DescriptionIsProvidedButEmpty -> ["Description is provided but empty."]
        | BusinessError e -> 
            match e with
            | Unauthorized -> ["Unauthorized."]
            | InvalidIngredients es -> List.map serializeCreateIngredientError es
        
    let private serializeCreate =
        Result.map serializeRecipe >> Result.mapError (Seq.collect serializeCreateError)

    let create accessToken parameters = 
        mkParameters parameters |> Validation.toResult |> Reader.id
        >>=! createRecipe accessToken

    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler createDao next ctx create serializeCreate
