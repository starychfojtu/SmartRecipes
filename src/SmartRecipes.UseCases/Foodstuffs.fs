namespace SmartRecipes.UseCases

open SmartRecipes.Domain

module Foodstuffs =
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes.IO
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.NonNegativeFloat

    // Get by ids
    
    type GetByIdsError = 
        | Unauthorized
        
    let getByIds accessToken ids = 
        Users.authorize Unauthorized accessToken
        >>= (fun _ -> Foodstuffs.getByIds ids |> IO.toSuccessEIO)
    
    // Search
    
    type SearchError = 
        | Unauthorized
        
    let search accessToken query =
        Users.authorize Unauthorized accessToken
        >>= fun _ -> Foodstuffs.search query |> IO.toSuccessEIO

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
        |> IO.toEIO (fun f -> 
            if Seq.isEmpty f
                then Ok foodstuff
                else Error FoodstuffAlreadyExists)

    let create accessToken parameters = 
        Users.authorize CreateError.Unauthorized accessToken
        >>= fun _ -> createFoodstuff parameters
        >>= ensureDoesntAlreadyExists
        >>= (Foodstuffs.add >> IO.toSuccessEIO)