namespace SmartRecipes.Api

module Foodstuffs =
    open Dto
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
            Ids: Guid list
        }
        
        type Response = {
            Foodstuffs: FoodstuffDto list
        }
            
        let private serialize = 
            Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs |> Seq.toList }) (function GetByIdsError.Unauthorized -> "Unauthorized.")
            
        let private getByIds accessToken parameters =
            Foodstuffs.getByIds accessToken parameters.Ids

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
            Result.bimap (fun fs -> { Foodstuffs = Seq.map Dto.serializeFoodstuff fs |> Seq.toList }) serializeError
            
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
        
        type Error = 
            | BusinessError of Foodstuffs.CreateError
            | BaseAmountError of AmountParameters.Error
            | AmountStepCannotBeNegative
            | NameCannotBeEmpty
            
        let private createParameters name baseAmount amountStep: Foodstuffs.CreateParameters = {
            name = name
            baseAmount = baseAmount
            amountStep = amountStep
        }
            
        let private parseBaseAmount =
            AmountParameters.parse >> Validation.mapFailure (List.map BaseAmountError)
        
        let private parse (parameters: Parameters) =
            createParameters
            <!> Parse.nonEmptyString [NameCannotBeEmpty] parameters.name
            <*> Parse.option parseBaseAmount parameters.baseAmount
            <*> Parse.option (Parse.nonNegativeFloat [AmountStepCannotBeNegative]) parameters.amountStep

        let private createFoodstuff token parameters =
            Foodstuffs.create token parameters |> ReaderT.mapError (fun e -> [BusinessError(e)])
            
        let private serializeError = function
            | NameCannotBeEmpty -> "Name cannot be empty."
            | AmountStepCannotBeNegative -> "Amount step cannot be negative."
            | BaseAmountError e ->
                match e with
                | AmountParameters.Error.UnitCannotBeEmpty -> "Unit cannot be empty."
                | AmountParameters.Error.ValueCannotBeNegative -> "Base amount cannot be negative."
            | BusinessError e ->
                match e with
                | Unauthorized -> "Unauthorized."
                | FoodstuffAlreadyExists -> "Foodstuff already exists."
            
        let private serialize<'a> =
            Result.bimap (fun f -> { Foodstuff = serializeFoodstuff f }) (Seq.map serializeError)

        let create token parameters =
            parse parameters |> toResult |> ReaderT.id 
             >>= createFoodstuff token

        let handler<'a> =
            authorizedPostHandler create serialize