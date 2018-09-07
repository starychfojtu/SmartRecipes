module UseCases.Foodstuffs
    open DataAccess
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Domain
    open Domain.Foodstuff
    open Domain.NonEmptyString
    open Infrastructure.Validation
    open Infrastructure.Reader
    open DataAccess.Foodstuffs
    open DataAccess.Tokens

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
        | NotAuthorized
        | FoodstuffAlreadyExists
        
    let private authorize accessToken =
        Users.authorize NotAuthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
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
        authorize accessToken
        >>=! fun _ -> createFoodstuff parameters
        >>=! ensureDoesntAlreadyExists
        >>=! addToDatabase