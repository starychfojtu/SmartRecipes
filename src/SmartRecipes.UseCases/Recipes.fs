module UseCases.Recipes
    open DataAccess
    open DataAccess.Foodstuffs
    open FSharpPlus.Data
    open Infrastructure
    open FSharpPlus
    open FSharpPlus.Data.Validation
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
    open Domain.NonNegativeFloat
    open Domain.Account
    open Domain.NonEmptyString
    open Domain.NaturalNumber
    open Domain.Recipe
    open Domain.Token
    open Domain.Foodstuff
    open Infrastructure
    open Infrastructure
                
    // Get all by account
    
    type GetMyRecipesDao = {
        tokens: TokensDao
        recipes: RecipesDao
    }
    
    type GetMyRecipesError =
        | Unauthorized
    
    let private authorize accessToken = 
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getRecipes accountId = 
        Reader(fun dao -> dao.recipes.getByAccount accountId |> Ok)
        
    let getMyRecipes accessToken =
        authorize accessToken
        >>=! getRecipes
        
    // Create
    
    type CreateRecipeDao = {
        tokens: TokensDao
        recipes: RecipesDao
        foodstuffs: FoodstuffDao
    }
    
    type CreateIngredientError = 
        | DuplicateFoodstuffIngredient
        | FoodstuffNotFound
    
    type CreateError =
        | Unauthorized
        | InvalidIngredients of CreateIngredientError list
        
    type CreateIngredientParameter = {
        foodstuffId: Guid
        amount: NonNegativeFloat
    }
    
    type CreateParameters = {
        name: NonEmptyString
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<CreateIngredientParameter>
    }
    
    let private authorizeCreate accessToken = 
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getFoodstuff parameters = 
        Reader(fun dao -> Seq.map (fun i -> i.foodstuffId) parameters |> dao.foodstuffs.getByIds )

    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredient foodstuffMap parameters =
        createIngredient
        <!> mkFoodstuffId parameters.foodstuffId foodstuffMap
        <*> (Success parameters.amount)
    
    let private mkIngredients parameters foodstuffMap =
        NonEmptyList.map (mkIngredient foodstuffMap) parameters |> Validation.traverseNonEmptyList
    
    let private checkIngredientsNotDuplicate ingredients =
        if NonEmptyList.isDistinct ingredients 
            then Success ingredients 
            else Failure [DuplicateFoodstuffIngredient]
            
    let private createIngredients parameters =
        let toMap = Seq.map (fun (f: Foodstuff) -> (f.id.value, f)) >> Map.ofSeq
        getFoodstuff parameters
        |> Reader.map toMap
        |> Reader.map (mkIngredients parameters)
        |> Reader.map (Validation.bind checkIngredientsNotDuplicate)
        |> Reader.map (Validation.toResult)
        |> Reader.map (Result.mapError InvalidIngredients)

    let private createRecipe parameters ingredients accountId = 
        Recipe.createRecipe parameters.name accountId parameters.personCount parameters.imageUrl parameters.description ingredients
        |> Ok 
        |> Reader.id

    let private addToDatabase recipe = 
        Reader(fun (dao: CreateRecipeDao) -> dao.recipes.add recipe |> Ok)

    let create accessToken parameters =
        authorizeCreate accessToken
        >>=! (fun a -> createIngredients parameters.ingredients |> Reader.map (Result.map (fun i -> (i, a)))) 
        >>=! (fun (ingredients, accountId) -> createRecipe parameters ingredients accountId)
        >>=! addToDatabase
        
    // Update 
    
    // TODO : Implement as Creating new one, but then just use old Id and call update instead
    
    // Delete
    
    // TODO