module UseCases.Recipes
    open Business
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Domain.Account
    open FSharpPlus
    open Infrastructure.Reader
    open Infrastructure.Option
    open System
    open UseCases
    open Users
    open DataAccess.Model
    open Domain.Token
    open Domain.Foodstuff
    open Infrastructure.Seq
    open DataAccess.Recipes
    open DataAccess.Tokens
    open Domain
    open Domain.NonEmptyString
    open Domain.NaturalNumber
                
    // Get all by account
    
    type GetByAccountDao = {
        tokens: TokensDao
        recipes: RecipesDao
    }
    
    type GetByAccountError =
        | Unauthorized
        | UserNotFound
    
    let private authorize accessToken = 
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getRecipes accountId = Reader(fun dao ->
        dao.recipes.getByAccount accountId |> Ok)
        
    let getAllbyAccount accessToken accountId =
        authorize accessToken
        >>=! fun _ -> getRecipes accountId
        
    // Creates
    
    type CreateError =
        | Unauthorized
        
    type IngredientParameter = {
        foodstuff: Foodstuff
        amount: float
    }
    
    type CreateParameters = {
        name: NonEmptyString
        creatorId: AccountId
        personCount: NaturalNumber
        imageUrl: Uri
        description: string
    }

    let private createRecipe infoParameters ingredientParameters token = 
        Recipes.create token.accountId infoParameters ingredientParameters 
        |> Result.mapError (List.map InvalidParameters)
        |> Reader.id

    let create accessToken parameters =
        authorize [Unauthorized] accessTokenValue
        >>=! mapParameters ingredientParameters
        >>=! (fun (t, ingredientParameters) -> createRecipe infoParameters ingredientParameters t)