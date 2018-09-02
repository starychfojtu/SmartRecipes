module UseCases.Foodstuffs
    open DataAccess
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Models
    open Models.Foodstuff
    open Models.NonEmptyString
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
        Users.authorize NotAuthorized accessToken
        >>=! (fun _ -> createFoodstuff parameters)
        >>=! ensureDoesntAlreadyExists
        >>=! addToDatabase