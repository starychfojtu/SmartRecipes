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
    open FSharpPlus.Data.Validation
    open Models.Foodstuff

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
    
    let createParameters name baseAmount amountStep: UseCases.Foodstuffs.CreateParameters = {
        name = name
        baseAmount = baseAmount
        amountStep = amountStep
    }
    
    let parseUnit = function
        | "gram" -> Success MetricUnit.Gram
        | "piece" -> Success MetricUnit.Piece
        | "liter" -> Success MetricUnit.Liter
        | _ -> Failure 
    
    let mkAmount value unit = 
        createAmount unit
        <!> mkNonNegativeFloat value
    
    let createAmount amount = 
        match amount = null with 
        | true -> Success None
        | false -> mk 
    
    let parseParameters (parameters: CreateParameters): UseCases.Foodstuffs.CreateParameters = 
        createParameters
        <!> mkNonEmptyString parameters.name
        <*>
        

    let createHandle (next: HttpFunc) (ctx: HttpContext) = 
        authorizedPostHandler<CreateParameters> next ctx (fun token parameters ->
            Foodstuffs.create )