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
        
    
    let toNonEmptyStringModel s =
        NonEmptyString.create s |> Option.get
        
    let toNaturalNumberModel n =
        NaturalNumber.create n |> Option.get
        
        
    let internal amountToDb (amount: Amount) : DbAmount = {
        unit = amount.unit.Value.Value
        value = NonNegativeFloat.value amount.value 
    }
    
    let internal amountToModel (dbAmount: DbAmount) = {
        unit = NonEmptyString.create dbAmount.unit |> Option.get |> MetricUnit
        value = NonNegativeFloat.create dbAmount.value |> Option.get
    }