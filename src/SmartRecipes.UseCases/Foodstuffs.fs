namespace SmartRecipes.UseCases
open SmartRecipes.Domain

module Foodstuffs =
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes.DataAccess
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.NonNegativeFloat

    // Get by ids
    
    type GetByIdsError = 
        | Unauthorized

    let private getFoodstuffsByIds ids =
        Foodstuffs.getByIds ids
        |> ReaderT.hoistOk
        
    let getByIds accessToken ids = 
        Users.authorize Unauthorized accessToken
        >>= (fun _ -> getFoodstuffsByIds ids)
    
    // Search
    
    type SearchError = 
        | Unauthorized
        
    let private searchFoodstuff query =
        Foodstuffs.search query
        |> ReaderT.hoistOk
        
    let private searchFoodstuff2 query =
        Foodstuffs.search query
        |> ReaderT.hoistOk
        
    let search accessToken query =
        Users.authorize Unauthorized accessToken
        >>= fun _ -> searchFoodstuff query

    // Create

    type CreateParameters = {
        name: NonEmptyString
        baseAmount: Amount option
        amountStep: NonNegativeFloat option
    }
    
    type CreateError = 
        | Unauthorized
        | FoodstuffAlreadyExists
        
    let private authorizeCreate accessToken =
         accessToken
        
    let private createFoodstuff parameters =
        createFoodstuff parameters.name parameters.baseAmount parameters.amountStep |> Ok |> ReaderT.id

    let private ensureDoesntAlreadyExists (foodstuff: Foodstuff) =
        Foodstuffs.search (SearchQuery.create foodstuff.name)
        |> Reader.map (fun f -> 
            if Seq.isEmpty f
                then Ok foodstuff
                else Error FoodstuffAlreadyExists
        )
        |> ReaderT.fromReader
        
    let private addToDatabase foodstuff = 
        Foodstuffs.add foodstuff |> ReaderT.hoistOk

    let create accessToken parameters = 
        Users.authorize CreateError.Unauthorized accessToken
        >>= fun _ -> createFoodstuff parameters
        >>= ensureDoesntAlreadyExists
        >>= addToDatabase