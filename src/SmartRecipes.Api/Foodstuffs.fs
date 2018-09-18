module Api.Foodstuffs
    open Api
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
    open Domain
    open Domain.Foodstuff
    open System
    open UseCases
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Foodstuffs
    open DataAccess.Tokens
    
    // Get by Ids
    
    [<CLIMutable>]
    type GetByIdsParameters = {
        ids: Guid list
    }
    
    type GetByIdsError = 
        | Unauthorized
        
    type GetByIdsDao = {
        tokens: TokensDao
        foodstuffs: FoodstuffDao
    }
    
    let private getByIdsDao () = {
        tokens = Tokens.getDao ()
        foodstuffs = Foodstuffs.getDao ()
    }
    
    let private authorize accessToken =
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
    
    let private getFoodstuffsByIds ids = 
        Reader(fun dao -> dao.foodstuffs.getByIds ids |> Ok)
        
    let private serializeGetByIds<'a, 'b> = 
        Result.map (Seq.map Dto.serializeFoodstuff) >> Result.mapError (function Unauthorized -> "Unauthorized.")
        
    let getByIds accessToken parameters = 
        authorize accessToken
        >>=! (fun _ -> getFoodstuffsByIds parameters.ids)
        
    let getByIdshandler ctx next = 
        authorizedGetHandler (getByIdsDao ()) ctx next getByIds serializeGetByIds
        
    // Search
    
    [<CLIMutable>]
    type SearchParameters = {
        query: string
    }
    
    type SearchError = 
        | Unauthorized
        | QueryIsEmpty
        
    type SearchDao = {
        tokens: TokensDao
        foodstuffs: FoodstuffDao
    }
    
    let private getSearchDao (): SearchDao = {
        tokens = Tokens.getDao ()
        foodstuffs = Foodstuffs.getDao ()
    }
    
    let private authorizeSearch accessToken =
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private mkQuery parameters =
        mkNonEmptyString parameters.query |> toResult |> Result.mapError (fun _ -> QueryIsEmpty) |> Reader.id
    
    let private searchFoodstuffs query = 
        Reader(fun dao -> dao.foodstuffs.search query |> Ok)
        
    let search accessToken parameters = 
        authorizeSearch accessToken
        >>=! (fun _ -> mkQuery parameters)
        >>=! searchFoodstuffs
        
    let searchHandler ctx next = 
        authorizedGetHandler (getSearchDao ()) ctx next search (fun a -> a)

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
    
    let private getDao (): CreateFoodstuffDao = {
        tokens = (Tokens.getDao ())
        foodstuffs = (Foodstuffs.getDao ())
    }
    
    let private parseUnit = function
        | "gram" -> FSharpPlus.Data.Validation.Success MetricUnit.Gram
        | "piece" -> FSharpPlus.Data.Validation.Success MetricUnit.Piece
        | "liter" -> FSharpPlus.Data.Validation.Success MetricUnit.Liter
        | _ -> FSharpPlus.Data.Validation.Failure [UnknownAmountUnit]
    
    let private mkAmount parameters =
        let parseValue value = value |> NonNegativeFloat.mkNonNegativeFloat |> Validation.mapFailure (fun _ -> [AmountCannotBeNegative])
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

    let create token parameters =
        parseParameters parameters |> toResult |> Reader.id 
         >>=! createFoodstuff token

    let createHandler (next: HttpFunc) (ctx: HttpContext) =
        authorizedPostHandler (getDao ()) next ctx create (fun a -> a)