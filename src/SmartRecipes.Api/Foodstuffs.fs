module Api.Foodstuffs
    open Api
    open Dto
    open Microsoft.AspNetCore.Http
    open System
    open Giraffe
    open Generic
    open Domain
    open UseCases
    open UseCases.Foodstuffs
    open Domain.NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure
    open Infrastructure.Reader
    open Domain.Foodstuff
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Tokens
    open Infrastracture
    
    // Get by Ids
    
    [<CLIMutable>]
    type GetByIdsParameters = {
        Ids: Guid list
    }
    
    type GetByIdsResponse = {
        Foodstuffs: FoodstuffDto seq
    }
    
    let private getByIdsDao: GetFoodstuffByIdsDao = {
        tokens = Tokens.dao
        foodstuffs = Foodstuffs.dao
    }
        
    let private serializeGetByIds = 
        Result.bimap (Seq.map Dto.serializeFoodstuff) (fun (e: GetByIdsError) -> match e with GetByIdsError.Unauthorized -> "Unauthorized.")
        
    let private getByIds accessToken parameters =
        UseCases.Foodstuffs.getByIds accessToken parameters.Ids

    let getByIdshandler ctx next = 
        authorizedGetHandler getByIdsDao ctx next getByIds serializeGetByIds
        
    // Search
    
    [<CLIMutable>]
    type SearchParameters = {
        query: string
    }
    
    type SearchError = 
        | BusinessError of UseCases.Foodstuffs.SearchError
        | QueryIsEmpty
    
    let private searchDao: SearchDao = {
        tokens = Tokens.dao
        foodstuffs = Foodstuffs.dao
    }
    
    let private mkQuery parameters =
        mkNonEmptyString parameters.query |> toResult |> Result.mapError (fun _ -> QueryIsEmpty) |> Reader.id
        
    let private serializeSearchError = function 
        | BusinessError e ->
            match e with
            | SearchError.Unauthorized -> "Unauthorized."
        | QueryIsEmpty -> "Query is empty."
        
    let private serializeSearch<'a, 'b> = 
        Result.bimap (Seq.map Dto.serializeFoodstuff) serializeSearchError
        
    let searchFoodstuffs accessToken query =
        UseCases.Foodstuffs.search accessToken query |> Reader.map (Result.mapError BusinessError)
        
    let search accessToken parameters = 
        mkQuery parameters
        >>=! searchFoodstuffs accessToken
        
    let searchHandler ctx next = 
        authorizedGetHandler searchDao ctx next search serializeSearch

    // Create

    [<CLIMutable>]
    type AmountParameters = {
        unit: string
        value: float
    }
        
    [<CLIMutable>]
    type CreateParameters = {
        name: string
        baseAmount: AmountParameters
        amountStep: AmountParameters
    }
    
    type CreateError = 
        | BusinessError of UseCases.Foodstuffs.CreateError
        | NameCannotBeEmpty
        | UnknownAmountUnit
        | AmountCannotBeNegative
    
    let private createParameters name baseAmount amountStep: UseCases.Foodstuffs.CreateParameters = {
        name = name
        baseAmount = baseAmount
        amountStep = amountStep
    }
    
    let private createFoodstuffDao: CreateFoodstuffDao = {
        tokens = Tokens.dao
        foodstuffs = Foodstuffs.dao
    }
    
    let private parseUnit = function
        | "gram" -> FSharpPlus.Data.Validation.Success MetricUnit.Gram
        | "piece" -> FSharpPlus.Data.Validation.Success MetricUnit.Piece
        | "liter" -> FSharpPlus.Data.Validation.Success MetricUnit.Liter
        | _ -> FSharpPlus.Data.Validation.Failure [UnknownAmountUnit]
    
    let private mkAmount parameters =
        let parseValue value = value |> NonNegativeFloat.create |> Validation.mapFailure (fun _ -> [AmountCannotBeNegative])
        createAmount
        <!> parseUnit parameters.unit
        <*> parseValue parameters.value
        |> map Some
        
    let private parseParameters (parameters: CreateParameters) =
        createParameters
        <!> safeMkNonEmptyString parameters.name |> Validation.mapFailure (fun _ -> [NameCannotBeEmpty])
        <*> mkAmount parameters.baseAmount
        <*> mkAmount parameters.amountStep

    let private createFoodstuff token parameters =
        Foodstuffs.create token parameters |> Reader.map (Result.mapError (fun e -> [BusinessError(e)]))
        
    let private serializeCreateError = function
        | NameCannotBeEmpty -> "Name cannot be empty."
        | UnknownAmountUnit -> "Unknown amount unit."
        | AmountCannotBeNegative -> "Amount cannot be negative."
        | BusinessError e ->
            match e with
            | NotAuthorized -> "Unauthorized."
            | FoodstuffAlreadyExists -> "Foodstuff already exists."
        
    let private serializeCreate =
        Result.map serializeFoodstuff >> Result.mapError (Seq.map serializeCreateError)

    let create token parameters =
        parseParameters parameters |> toResult |> Reader.id 
         >>=! createFoodstuff token

    let createHandler (next: HttpFunc) (ctx: HttpContext) =
        authorizedPostHandler createFoodstuffDao next ctx create serializeCreate