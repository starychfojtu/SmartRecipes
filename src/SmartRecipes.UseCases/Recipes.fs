module UseCases.Recipes
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open FSharpPlus
    open Infrastructure.Reader
    open Infrastructure.Option
    open System
    open UseCases
    open Users
    open DataAccess.Model
    open Infrastructure.Seq
    open DataAccess.Recipes
    open DataAccess.Tokens
    open Domain
    open Domain.Account
    open Domain.NonEmptyString
    open Domain.NaturalNumber
    open Domain.Recipe
    open Domain.Token
    open Domain.Foodstuff
                
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
        
    // Create
    
    type CreateRecipeDao = {
        tokens: TokensDao
        recipes: RecipesDao
    }
    
    type CreateError =
        | Unauthorized
        | DuplicateFoodstuffIngredient
    
    type CreateParameters = {
        name: NonEmptyString
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<IngredientParameter>
    }
    
    let private authorizeCreation accessToken = 
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private checkIngredientsNotDuplicate ingredientParameters accessToken =
        if NonEmptyList.isDistinct ingredientParameters then Ok accessToken else Error DuplicateFoodstuffIngredient
        |> Reader.id

    let private createRecipe parameters token = 
        Recipe.createRecipe parameters.name token.accountId parameters.personCount parameters.imageUrl parameters.description parameters.ingredients
        |> Ok 
        |> Reader.id

    let private addToDatabase recipe = 
        Reader(fun (dao: CreateRecipeDao) -> dao.recipes.add recipe |> Ok)

    let create accessToken parameters =
        authorizeCreation accessToken
        >>=! checkIngredientsNotDuplicate parameters.ingredients
        >>=! createRecipe parameters
        >>=! addToDatabase