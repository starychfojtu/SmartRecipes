namespace SmartRecipes.UseCases

module Foodstuffs =
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes.DataAccess.Foodstuffs
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open Environment

    // Get by ids
    
    type GetByIdsError = 
        | Unauthorized

    let private getFoodstuffsByIds ids = 
        ReaderT(fun env -> env.IO.Foodstuffs.getByIds ids |> Ok)
        
    let getByIds accessToken ids = 
        Users.authorize Unauthorized accessToken
        >>= (fun _ -> getFoodstuffsByIds ids)
    
    // Search
    
    type SearchError = 
        | Unauthorized
        
    let private searchFoodstuff query = 
        ReaderT(fun env -> env.IO.Foodstuffs.search query |> Ok)
        
    let search accessToken query =
        Users.authorize Unauthorized accessToken
        >>= fun _ -> searchFoodstuff query

    // Create

    type CreateParameters = {
        name: NonEmptyString
        baseAmount: Amount option
        amountStep: Amount option
    }
    
    type CreateError = 
        | Unauthorized
        | FoodstuffAlreadyExists
        
    let private authorizeCreate accessToken =
         accessToken
        
    let private createFoodstuff parameters =
        createFoodstuff parameters.name parameters.baseAmount parameters.amountStep |> Ok |> ReaderT.id

    let private ensureDoesntAlreadyExists (foodstuff: Foodstuff) = ReaderT(fun env ->
        let foodstuffsWithSameName = env.IO.Foodstuffs.search foodstuff.name
        if Seq.isEmpty foodstuffsWithSameName
            then Ok foodstuff
            else Error FoodstuffAlreadyExists
    )
        
    let private addToDatabase foodstuff = 
        ReaderT(fun env -> env.IO.Foodstuffs.add foodstuff |> Ok)

    let create accessToken parameters = 
        Users.authorize CreateError.Unauthorized accessToken
        >>= fun _ -> createFoodstuff parameters
        >>= ensureDoesntAlreadyExists
        >>= addToDatabase