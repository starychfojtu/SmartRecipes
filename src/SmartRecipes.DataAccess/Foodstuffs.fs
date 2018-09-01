module DataAccess.Foodstuffs
    open System
    open DataAccess.Context
    open DataAccess.Model
    open FSharpPlus.Data
    open Models.Foodstuff
    open Models.Foodstuff
    open Models.NonEmptyString
    open Models.NonNegativeFloat
    open Infrastructure.Validation
    
    let private unitToDb = function
        | MetricUnit.Liter -> DbMetricUnit.Liter
        | MetricUnit.Gram -> DbMetricUnit.Gram
        | MetricUnit.Piece -> DbMetricUnit.Piece
        
    let private unitToModel = function 
        | DbMetricUnit.Liter -> MetricUnit.Liter
        | DbMetricUnit.Gram -> MetricUnit.Gram
        | DbMetricUnit.Piece -> MetricUnit.Piece
        | _ -> raise (InvalidOperationException("Invalid unit value"))
        
    let private amountToDb amount : DbAmount = {
        unit = unitToDb amount.unit
        value = amount.value.value
    }
    
    let private amountToModel (dbAmount: DbAmount) = {
        unit = unitToModel dbAmount.unit
        value = mkNonNegativeFloat dbAmount.value |> forceSucces
    }
                
    let private toDb foodstuff : DbFoodstuff = {
        id = match foodstuff.id with FoodstuffId id -> id
        name = foodstuff.name.value
        baseAmount = amountToDb foodstuff.baseAmount
        amountStep = amountToDb foodstuff.amountStep
    }
    
    let private toModel (dbFoodstuff: DbFoodstuff) = {
        id = FoodstuffId dbFoodstuff.id 
        name = mkNonEmptyString dbFoodstuff.name |> forceSucces
        baseAmount = amountToModel dbFoodstuff.baseAmount
        amountStep = amountToModel dbFoodstuff.amountStep
    }
    
    // public
    
    let getByIds ids = Reader(fun (ctx: Context) ->
        ctx.Foodstuffs
        |> Seq.where (fun f -> Seq.contains f.id ids)
        |> Seq.map toModel
    )
    
    let search (name: NonEmptyString) = Reader(fun (ctx: Context) ->
        ctx.Foodstuffs
        |> Seq.where (fun f -> f.name = name.value)
        |> Seq.map toModel
    )
    
    let add foodstuff = Reader(fun (ctx: Context) ->
        let dbModel = toDb foodstuff
        ctx.Add(dbModel) |> ignore
        ctx.SaveChanges() |> ignore
        foodstuff
    )