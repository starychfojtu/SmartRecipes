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
            
    // Get my recipes
    
    let private getMyRecipesDao (): GetMyRecipesDao = {
        tokens = (Tokens.getDao ())
        recipes = (Recipes.getDao ())
    }
    
    let private serializeGetMyRecipes = 
        Result.map (Seq.map serializeRecipe) >> Result.mapError (function Recipes.GetMyRecipesError.Unauthorized -> "Unauthorized.")
    
    let getMyRecipes accessToken parameters = 
        Recipes.getMyRecipes accessToken
    
    let getMyRecipesHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler (getMyRecipesDao ()) next ctx getMyRecipes serializeGetMyRecipes
            
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
        | FoodstuffNotFound
        | DescriptionIsProvidedButEmpty
        | BusinessError of Recipes.CreateError
        
    type CreateDao = {
        foodstuffs: FoodstuffDao
        recipes: RecipesDao
        tokens: TokensDao
    }
        
    let private getCreateDao () = {
        foodstuffs = Foodstuffs.getDao ()
        recipes = Recipes.getDao ()
        tokens = Tokens.getDao ()
    }
        
    let private createParameters name personCount imageUrl description ingredients: Recipes.CreateParameters = {
        name = name
        personCount = personCount
        imageUrl = imageUrl
        description = description
        ingredients = ingredients
    }
    
    let private getFoodstuff parameters = 
        Reader(fun (dao: FoodstuffDao) -> Seq.map (fun i -> i.foodstuffId) parameters |> dao.getByIds )

    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredients foodstuffMap parameters =
        createIngredient
        <!> mkFoodstuffId parameters.foodstuffId foodstuffMap
        <*> (NonNegativeFloat.create parameters.amount |> mapFailure (fun _ -> [AmountOfIngredientMustBePositive]))
        
    let private mkAllIngredients parameters foodstuffMap =
        Seq.map (mkIngredients foodstuffMap) parameters
        
    let private mkNonEmptyParameters parameters = 
        mkNonEmptyList parameters |> mapFailure (fun _ -> [MustContaintAtLeastOneIngredient])

    let private parseIngredientParameters parameters =
        let toMap = Seq.map (fun (f: Foodstuff) -> (f.id.value, f)) >> Map.ofSeq
        let parseIngredients = mkAllIngredients parameters >> Validation.traverse
        getFoodstuff parameters
        |> Reader.map toMap
        |> Reader.map parseIngredients
        |> Reader.map (Validation.bind mkNonEmptyParameters)

    let private mkDescription d =
        if isNull d
            then Success None 
            else mkNonEmptyString d |> mapFailure (fun _ -> [DescriptionIsProvidedButEmpty]) |> Validation.map Some 
        
    let private mkParameters (parameters: CreateParameters) = Reader(fun (dao: FoodstuffDao) ->
        createParameters
        <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
        <*> (NaturalNumber.create parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> mkDescription parameters.description
        <*> (parseIngredientParameters parameters.ingredients |> Reader.execute dao)
    )
    
    let private mapDao dao: CreateRecipeDao = {
        recipes = dao.recipes
        tokens = dao.tokens
    }
    
    let private parseParameters = mkParameters >> (Reader.map Validation.toResult) >> (Reader.mapEnviroment (fun dao -> dao.foodstuffs))
    let private createRecipe accessToken = 
        Recipes.create accessToken >> 
        (Reader.mapEnviroment mapDao) >> 
        (Reader.map (Result.mapError (fun e -> [BusinessError e])))
        
    let private serializeCreateError = function 
        | NameCannotBeEmpty -> "Name cannot be empty."
        | PersonCountMustBePositive -> "Person count must be positive."
        | InvalidImageUrl s -> s
        | AmountOfIngredientMustBePositive -> "Amount of ingredient must be positive."
        | MustContaintAtLeastOneIngredient -> "Must containt at least one ingredient."
        | FoodstuffNotFound -> "Foodstuff not found."
        | DescriptionIsProvidedButEmpty -> "Description is provided but empty."
        | BusinessError e -> 
            match e with
            | Unauthorized -> "Unauthorized."
            | DuplicateFoodstuffIngredient -> "Multiple ingredients with common foodstuff found."
        
        
    let private serializeCreate =
        Result.map serializeRecipe >> Result.mapError (Seq.map serializeCreateError)

    let create accessToken parameters = parseParameters parameters >>=! createRecipe accessToken

    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler (getCreateDao ()) next ctx create serializeCreate
