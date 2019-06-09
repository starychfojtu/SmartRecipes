namespace SmartRecipes.Api

open FSharpPlus.Data
open Infrastructure
open SmartRecipes.Domain
open SmartRecipes.Domain.Foodstuff
open SmartRecipes.Api
open FSharpPlus

module AmountParameters =
    [<CLIMutable>]
    type AmountParameters = {
        unit: string
        value: float
    }

    type Error =
        | UnitCannotBeEmpty
        | ValueCannotBeNegative

    let private parseUnit =
        Parse.nonEmptyString [UnitCannotBeEmpty] >> Validation.map MetricUnit
        
    let parse parameters =
        Foodstuff.createAmount
        <!> parseUnit parameters.unit
        <*> Parse.nonNegativeFloat [ValueCannotBeNegative] parameters.value

