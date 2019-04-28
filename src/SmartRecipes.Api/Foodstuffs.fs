namespace SmartRecipes.Api

module Foodstuffs =
    open Dto
    open System
    open Generic
    open SmartRecipes.Domain
    open SmartRecipes.Domain.NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open SmartRecipes.Domain.Foodstuff
    open Infrastracture
    open Infrastructure
    open SmartRecipes.DataAccess
    open SmartRecipes.UseCases
    open SmartRecipes.UseCases.Foodstuffs
    open SmartRecipes
    
    // Get by Ids
    
    [<CLIMutable>]
    type GetByIdsParameters = {
        Ids: Guid list
    }
    
    type GetByIdsResponse = {
        Foodstuffs: FoodstuffDto seq
    }
        
    let private serializeGetByIds = 
        Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs }) (fun e -> match e with GetByIdsError.Unauthorized -> "Unauthorized.")
        
    let private getByIds accessToken parameters =
        Foodstuffs.getByIds accessToken parameters.Ids

    let getByIdshandler<'a> = 
        authorizedGetHandler getByIds serializeGetByIds
        
    // Search
    
    [<CLIMutable>]
    type SearchParameters = {
        query: string
    }
    
    type SearchResponse = {
        Foodstuffs: FoodstuffDto seq
    }
    
    type SearchError = 
        | BusinessError of UseCases.Foodstuffs.SearchError
        | QueryIsEmpty
        
    let private serializeSearchError = function 
        | BusinessError e ->
            match e with
            | SearchError.Unauthorized -> "Unauthorized."
        | QueryIsEmpty -> "Query is empty."
        
    let private serializeSearch<'a, 'b> = 
        Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs }) serializeSearchError
        
    let private mkQuery parameters =
        mkNonEmptyString parameters.query |> toResult |> Result.mapError (fun _ -> QueryIsEmpty) |> ReaderT.id
        
    let searchFoodstuffs accessToken query =
        Foodstuffs.search accessToken query |> ReaderT.mapError BusinessError
        
    let search accessToken parameters = 
        mkQuery parameters
        >>= searchFoodstuffs accessToken
        
    let searchHandler<'a> = 
        authorizedGetHandler search serializeSearch

    // Create

    [<CLIMutable>]
    type AmountParameters = {
        unit: string
        value: float
    }
    
    type CreateResponse = {
        Foodstuff: FoodstuffDto
    }
        
    [<CLIMutable>]
    type CreateParameters = {
        name: string
        baseAmount: AmountParameters
        amountStep: float
    }
    
    type CreateError = 
        | BusinessError of Foodstuffs.CreateError
        | NameCannotBeEmpty
        | UnknownAmountUnit
        | AmountCannotBeNegative
    
    let private createParameters name baseAmount amountStep: Foodstuffs.CreateParameters = {
        name = name
        baseAmount = baseAmount
        amountStep = amountStep
    }
    
    let private parseUnit = function
        | "gram" -> Validation.Success MetricUnit.Gram
        | "piece" -> Validation.Success MetricUnit.Piece
        | "liter" -> Validation.Success MetricUnit.Liter
        | _ -> Validation.Failure [UnknownAmountUnit]
        
    let private parseValue =
        NonNegativeFloat.create >> Validation.mapFailure (fun _ -> [AmountCannotBeNegative])
    
    let private mkAmount parameters =
        createAmount
        <!> parseUnit parameters.unit
        <*> parseValue parameters.value
        
    let private parseParameters (parameters: CreateParameters) =
        createParameters
        <!> safeMkNonEmptyString parameters.name |> Validation.mapFailure (fun _ -> [NameCannotBeEmpty])
        <*> (mkAmount parameters.baseAmount |> map Some)
        <*> (parseValue parameters.amountStep |> map Some)

    let private createFoodstuff token parameters =
        Foodstuffs.create token parameters |> ReaderT.mapError (fun e -> [BusinessError(e)])
        
    let private serializeCreateError = function
        | NameCannotBeEmpty -> "Name cannot be empty."
        | UnknownAmountUnit -> "Unknown amount unit."
        | AmountCannotBeNegative -> "Amount cannot be negative."
        | BusinessError e ->
            match e with
            | NotAuthorized -> "Unauthorized."
            | FoodstuffAlreadyExists -> "Foodstuff already exists."
        
    let private serializeCreate<'a> =
        Result.bimap (fun f -> { Foodstuff = serializeFoodstuff f }) (Seq.map serializeCreateError)

    let create token parameters =
        parseParameters parameters |> toResult |> ReaderT.id 
         >>= createFoodstuff token

    let createHandler<'a> =
        authorizedPostHandler create serializeCreate