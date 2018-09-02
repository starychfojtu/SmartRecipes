module Api.Foodstuffs
    open Api
    open Microsoft.AspNetCore.Http
    open System
    open Giraffe
    open Generic
    open Models
    open UseCases
    open UseCases.Foodstuffs
    open Models.NonEmptyString
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure
    open Infrastructure.Reader
    open Models
    open Models.Foodstuff
    open System
    open UseCases

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
        let dao = (DataAccess.Foodstuffs.getDao ())
        Foodstuffs.create dao token parameters |> Reader.map (Result.mapError (fun e -> [BusinessError(e)]))

    let createHandler (next: HttpFunc) (ctx: HttpContext) =
        authorizedPostHandler next ctx (fun parameters token ->
            parseParameters parameters |> toResult |> Reader.id 
            >>=! createFoodstuff token )