namespace SmartRecipes.Api

module Foodstuffs =
    open Dto
    open Errors
    open System
    open Generic
    open SmartRecipes.Domain
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastracture
    open SmartRecipes.DataAccess
    open SmartRecipes.UseCases
    open SmartRecipes.UseCases.Foodstuffs
    open SmartRecipes
    open AmountParameters
    
    module GetByIds = 
        [<CLIMutable>]
        type Parameters = {
            ids: Guid list
        }
        
        type Response = {
            Foodstuffs: FoodstuffDto list
        }
            
        let private serialize = 
            Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs |> Seq.toList }) (function GetByIdsError.Unauthorized -> error "Unauthorized.")
            
        let private getByIds accessToken parameters =
            Foodstuffs.getByIds accessToken parameters.ids

        let handler<'a> = 
            authorizedGetHandler getByIds serialize
            
    module Search = 
        open Infrastructure
        
        [<CLIMutable>]
        type Parameters = {
            query: string
        }
    
        type Response = {
            Foodstuffs: FoodstuffDto list
        }
        
        type Error = 
            | BusinessError of UseCases.Foodstuffs.SearchError
            | QueryIsEmpty
            
        let private serializeError = function 
            | BusinessError e ->
                match e with
                | SearchError.Unauthorized -> "Unauthorized."
            | QueryIsEmpty -> "Query is empty."
            
        let private serializeSearch<'a, 'b> = 
            Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs |> Seq.toList }) (serializeError >> error)
            
        let private parseQuery parameters =
            Parse.nonEmptyString QueryIsEmpty parameters.query 
            |> Validation.map SearchQuery.create
            |> Validation.toResult 
            |> ReaderT.id
            
        let private searchFoodstuffs accessToken query =
            Foodstuffs.search accessToken query |> ReaderT.mapError BusinessError
            
        let private search accessToken parameters = 
            parseQuery parameters
            >>= searchFoodstuffs accessToken
            
        let handler<'a> = 
            authorizedGetHandler search serializeSearch

    module Create = 
        open Infrastructure
        
        [<CLIMutable>]
        type Parameters = {
            name: string
            baseAmount: AmountParameters option
            amountStep: float option
        }
        
        type Response = {
            Foodstuff: FoodstuffDto
        }
        
        type ParameterError =
            | BaseAmountError of AmountParameters.Error
            | AmountStepCannotBeNegative
            | NameCannotBeEmpty
        
        type Error = 
            | BusinessError of Foodstuffs.CreateError
            | ParameterErrors of ParameterError list
            
        let private createParameters name baseAmount amountStep: Foodstuffs.CreateParameters = {
            name = name
            baseAmount = baseAmount
            amountStep = amountStep
        }
            
        let private parseBaseAmount =
            AmountParameters.parse >> Validation.mapFailure (List.map BaseAmountError)
        
        let private parse (parameters: Parameters) =
            let parsedParameters = 
                createParameters
                <!> Parse.nonEmptyString [NameCannotBeEmpty] parameters.name
                <*> Parse.option parseBaseAmount parameters.baseAmount
                <*> Parse.option (Parse.nonNegativeFloat [AmountStepCannotBeNegative]) parameters.amountStep
                
            parsedParameters |> toResult |> Result.mapError ParameterErrors |> ReaderT.id 

        let private createFoodstuff token parameters =
            Foodstuffs.create token parameters |> ReaderT.mapError BusinessError
            
        let private serializeParameterError = function
            | NameCannotBeEmpty -> { message =  "Cannot be empty."; parameter = "Name" }
            | AmountStepCannotBeNegative -> { message =  "Cannot be negative."; parameter = "Amount step" }
            | BaseAmountError e ->
                match e with
                | AmountParameters.Error.UnitCannotBeEmpty -> { message =  "Cannot be empty."; parameter = "Unit" }
                | AmountParameters.Error.ValueCannotBeNegative -> { message =  "Cannot be negative."; parameter = "Base amount" }
                
        let private serializaBusinessError = function
            | Unauthorized -> "Unauthorized."
            | FoodstuffAlreadyExists -> "Foodstuff already exists."
            
        let private serializeError = function
            | ParameterErrors es -> List.map serializeParameterError es |> invalidParameters
            | BusinessError e -> serializaBusinessError e |> error 
                
        let private serialize<'a> =
            Result.bimap (fun f -> { Foodstuff = serializeFoodstuff f }) (serializeError)

        let create token parameters =
            parse parameters
             >>= createFoodstuff token

        let handler<'a> =
            authorizedPostHandler create serialize