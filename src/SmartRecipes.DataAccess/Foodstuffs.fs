module DataAccess.Foodstuffs
    open System
    open DataAccess.Context
    open DataAccess.Model
    open FSharpPlus.Data
    open Models.Foodstuff
    open Models.Foodstuff
    open Models.NonEmptyString
    open Models.NonNegativeFloat
    
    let private unitToDb = function
        | MetricUnit.Liter -> DbMetricUnit.Liter
        | MetricUnit.Gram -> DbMetricUnit.Gram
        | MetricUnit.Piece -> DbMetricUnit.Piece
        
    let private unitToModel = function 
        | DbMetricUnit.Liter -> MetricUnit.Liter
        | DbMetricUnit.Gram -> MetricUnit.Gram
        | DbMetricUnit.Piece -> MetricUnit.Piece
        | _ -> raise (InvalidOperationException("Invalid enum value"))
        
    let private amountToDb amount : DbAmount = {
        unit = unitToDb amount.unit
        value = match amount.value with NonNegativeFloat v -> v
    }
    
    let private amountToModel (dbAmount: DbAmount) = {
        unit = unitToModel dbAmount.unit
        value = NonNegativeFloat dbAmount.value 
    }
                
    let private toDb foodstuff : DbFoodstuff = {
        id = match foodstuff.id with FoodstuffId id -> id
        name = match foodstuff.name with NonEmptyString n -> n
        baseAmount = amountToDb foodstuff.baseAmount
        amountStep = amountToDb foodstuff.amountStep
    }
    
    let private toModel (dbFoodstuff: DbFoodstuff) = {
        id = FoodstuffId dbFoodstuff.id 
        name = NonEmptyString dbFoodstuff.name
        baseAmount = amountToModel dbFoodstuff.baseAmount
        amountStep = amountToModel dbFoodstuff.amountStep
    }
    
    let getByIds ids = Reader(fun (ctx: Context) ->
        ctx.Foodstuffs
        |> Seq.where (fun f -> Seq.contains f.id ids)
        |> Seq.map toModel
    )