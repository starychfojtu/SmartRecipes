module UseCases.Foodstuffs
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Domain.Foodstuff
    open Domain.NonEmptyString
    open Infrastructure.Reader
    open DataAccess.Foodstuffs
    open DataAccess.Tokens

    // Get by ids
    
    type GetByIdsError = 
        | Unauthorized
        
    type GetFoodstuffByIdsDao = {
        tokens: TokensDao
        foodstuffs: FoodstuffDao
    }

    let private authorize accessToken =
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
    
    let private getFoodstuffsByIds ids = 
        Reader(fun dao -> dao.foodstuffs.getByIds ids |> Ok)
        
    let getByIds accessToken ids = 
        authorize accessToken
        >>=! (fun _ -> getFoodstuffsByIds ids)
    
    // Search
    
    type SearchError = 
        | Unauthorized
    
    type SearchDao = {
        tokens: TokensDao
        foodstuffs: FoodstuffDao
    }
    
    let private authorizeSearch accessToken =
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
    
    let private searchFoodstuff query = 
        Reader(fun dao -> dao.foodstuffs.search query |> Ok)
        
    let search accessToken query =
        authorizeSearch accessToken
        >>=! fun _ -> searchFoodstuff query

    // Create
    
    type CreateFoodstuffDao = {
        tokens: TokensDao
        foodstuffs: FoodstuffDao
    }

    type CreateParameters = {
        name: NonEmptyString
        baseAmount: Amount option
        amountStep: Amount option
    }
    
    type CreateError = 
        | Unauthorized
        | FoodstuffAlreadyExists
        
    let private authorizeCreate accessToken =
        Users.authorize CreateError.Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private createFoodstuff parameters =
        createFoodstuff parameters.name parameters.baseAmount parameters.amountStep |> Ok |> Reader.id

    let private ensureDoesntAlreadyExists (foodstuff: Foodstuff) = Reader(fun (dao: CreateFoodstuffDao) ->
        let foodstuffsWithSameName = dao.foodstuffs.search foodstuff.name
        if Seq.isEmpty foodstuffsWithSameName
            then Ok foodstuff
            else Error FoodstuffAlreadyExists
    )
        
    let private addToDatabase foodstuff = 
        Reader(fun (dao: CreateFoodstuffDao) -> dao.foodstuffs.add foodstuff |> Ok)

    let create accessToken parameters = 
        authorizeCreate accessToken
        >>=! fun _ -> createFoodstuff parameters
        >>=! ensureDoesntAlreadyExists
        >>=! addToDatabase