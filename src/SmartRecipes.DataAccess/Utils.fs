namespace SmartRecipes.DataAccess

module Utils =
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open System

    let forceSucces = function
        | Success s -> s
        | Failure f -> raise (InvalidOperationException("Invalid data in database."))


    let toNonEmptyStringModel =
        NonEmptyString.create >> Option.get

    let toNaturalNumberModel =
        NaturalNumber.create >> Option.get

    let toNonNegativeFloatModel =
        NonNegativeFloat.create >> Option.get

    let internal amountToDb (amount: Amount) =
        DbAmount(
            amount.unit.Value.Value,
            NonNegativeFloat.value amount.value
        )

    let internal amountToModel (dbAmount: DbAmount) = {
        unit = NonEmptyString.create dbAmount.unit |> Option.get |> MetricUnit
        value = NonNegativeFloat.create dbAmount.value |> Option.get
    }