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
        
    let private parseQuery parameters =
        Parse.nonEmptyString QueryIsEmpty parameters.query 
        |> Validation.map SearchQuery.create
        |> Validation.toResult 
        |> ReaderT.id
        
    let searchFoodstuffs accessToken query =
        Foodstuffs.search accessToken query |> ReaderT.mapError BusinessError
        
    let search accessToken parameters = 
        parseQuery parameters
        >>= searchFoodstuffs accessToken
        
    let searchHandler<'a> = 
        authorizedGetHandler search serializeSearch

    // Create

    [<CLIMutable>]
    type AmountParameters = {
        unit: string
        value: float
    }
    
    type ParseAmountError =
        | UnitCannotBeEmpty
        | ValueCannotBeNegative
        
    module AmountParameters =
        let private parseUnit =
            Parse.nonEmptyString [UnitCannotBeEmpty] >> Validation.map MetricUnit
            
        let parse parameters =
            createAmount
            <!> parseUnit parameters.unit
            <*> Parse.nonNegativeFloat [ValueCannotBeNegative] parameters.value
    
    type CreateResponse = {
        Foodstuff: FoodstuffDto
    }
        
    [<CLIMutable>]
    type CreateParameters = {
        name: string
        baseAmount: AmountParameters option
        amountStep: float option
    }
    
    type CreateError = 
        | BusinessError of Foodstuffs.CreateError
        | BaseAmountError of ParseAmountError
        | AmountStepCannotBeNegative
        | NameCannotBeEmpty
        
    module CreateParameters = 
        let private create name baseAmount amountStep: Foodstuffs.CreateParameters = {
            name = name
            baseAmount = baseAmount
            amountStep = amountStep
        }
            
        let private parseBaseAmount =
            AmountParameters.parse >> Validation.mapFailure (List.map BaseAmountError)
        
        let parse (parameters: CreateParameters) =
            create
            <!> Parse.nonEmptyString [NameCannotBeEmpty] parameters.name
            <*> Parse.option parseBaseAmount parameters.baseAmount
            <*> Parse.option (Parse.nonNegativeFloat [AmountStepCannotBeNegative]) parameters.amountStep

    let private createFoodstuff token parameters =
        Foodstuffs.create token parameters |> ReaderT.mapError (fun e -> [BusinessError(e)])
        
    let private serializeCreateError = function
        | NameCannotBeEmpty -> "Name cannot be empty."
        | AmountStepCannotBeNegative -> "Amount step cannot be negative."
        | BaseAmountError e ->
            match e with
            | ParseAmountError.UnitCannotBeEmpty -> "Unit cannot be empty."
            | ParseAmountError.ValueCannotBeNegative -> "Base amount cannot be negative."
        | BusinessError e ->
            match e with
            | Unauthorized -> "Unauthorized."
            | FoodstuffAlreadyExists -> "Foodstuff already exists."
        
    let private serializeCreate<'a> =
        Result.bimap (fun f -> { Foodstuff = serializeFoodstuff f }) (Seq.map serializeCreateError)

    let create token parameters =
        CreateParameters.parse parameters |> toResult |> ReaderT.id 
         >>= createFoodstuff token

    let createHandler<'a> =
        authorizedPostHandler create serializeCreate