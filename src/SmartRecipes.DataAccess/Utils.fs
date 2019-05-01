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
        NonEmptyString.create s |> forceSucces
        
    let toNaturalNumberModel n =
        NaturalNumber.create n |> forceSucces
        
        
    let internal amountToDb (amount: Amount) : DbAmount = {
        unit = amount.unit
        value = NonNegativeFloat.value amount.value 
    }
    
    let internal amountToModel (dbAmount: DbAmount) = {
        unit = dbAmount.unit
        value = NonNegativeFloat.create dbAmount.value |> forceSucces
    }