module Api.Recipes
    open Api
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
    open Context
    open DataAccess.Foodstuffs
    open DataAccess.Recipes
    open DataAccess.Tokens
    open Domain
    open Domain.Foodstuff
    open Domain.FoodstuffAmount
    open Domain.Recipe
    open FSharpPlus.Data
    open Infrastructure
    open UseCases.Recipes
    open Infrastructure.Validation
    open NaturalNumber
    open Uri
    open NonNegativeFloat
    open FSharpPlus
    open Infrastructure
    open Infrastructure.NonEmptyList
    open Infrastructure.Reader
            
    // Get by account        
    
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let private getGetByAccountDao (): GetByAccountDao = {
        tokens = (Tokens.getDao ())
        recipes = (Recipes.getDao ())
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler (getGetByAccountDao ()) next ctx (fun token p ->
            Recipes.getAllbyAccount token p.accountId)
            
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

    let mkFoodstuff guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredients foodstuffMap parameters =
        createFoodstuffAmount
        <!> mkFoodstuff parameters.foodstuffId foodstuffMap
        <*> (mkNonNegativeFloat parameters.amount |> mapFailure (fun _ -> [AmountOfIngredientMustBePositive]))
        
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
        <*> (mkNaturalNumber parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> mkDescription parameters.description
        <*> (parseIngredientParameters parameters.ingredients |> Reader.execute dao)
    )
    
    let private mapDao dao: CreateRecipeDao = {
        recipes = dao.recipes
        tokens = dao.tokens
    }
    
    let parseParameters = mkParameters >> (Reader.map Validation.toResult) >> (Reader.mapEnviroment (fun dao -> dao.foodstuffs))
    let createRecipe accessToken = 
        Recipes.create accessToken >> 
        (Reader.mapEnviroment mapDao) >> 
        (Reader.map (Result.mapError (fun e -> [BusinessError e])))

    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler (getCreateDao ()) next ctx (fun accessToken parameters ->
            parseParameters parameters >>=! createRecipe accessToken
        )
