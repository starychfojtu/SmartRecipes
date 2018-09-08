module DataAccess.Foodstuffs
    open System
    open DataAccess.Context
    open DataAccess.Model
    open FSharpPlus.Data
    open Domain.Foodstuff
    open Domain.Foodstuff
    open Domain.NonEmptyString
    open Domain.NonNegativeFloat
    open Infrastructure.Validation
    
    type FoodstuffDao = {
        getByIds: seq<Guid> -> seq<Foodstuff>
        search: NonEmptyString -> seq<Foodstuff>
        add: Foodstuff -> Foodstuff
    }
    
    let private unitToDb = function
        | MetricUnit.Liter -> DbMetricUnit.Liter
        | MetricUnit.Gram -> DbMetricUnit.Gram
        | MetricUnit.Piece -> DbMetricUnit.Piece
        
    let private unitToModel = function 
        | DbMetricUnit.Liter -> MetricUnit.Liter
        | DbMetricUnit.Gram -> MetricUnit.Gram
        | DbMetricUnit.Piece -> MetricUnit.Piece
        | _ -> raise (InvalidOperationException("Invalid unit value"))
        
    let internal amountToDb amount : DbAmount = {
        unit = unitToDb amount.unit
        value = amount.value.value
    }
    
    let internal amountToModel (dbAmount: DbAmount) = {
        unit = unitToModel dbAmount.unit
        value = mkNonNegativeFloat dbAmount.value |> forceSucces
    }
                
    let internal toDb foodstuff : DbFoodstuff = {
        id = match foodstuff.id with FoodstuffId id -> id
        name = foodstuff.name.value
        baseAmount = amountToDb foodstuff.baseAmount
        amountStep = amountToDb foodstuff.amountStep
    }
    
    let internal toModel (dbFoodstuff: DbFoodstuff) = {
        id = FoodstuffId dbFoodstuff.id 
        name = mkNonEmptyString dbFoodstuff.name |> forceSucces
        baseAmount = amountToModel dbFoodstuff.baseAmount
        amountStep = amountToModel dbFoodstuff.amountStep
    }
    
    let private getByIds ids =
        createDbContext().Foodstuffs
        |> Seq.where (fun f -> Seq.contains f.id ids)
        |> Seq.map toModel
    
    let private search (name: NonEmptyString) =
        createDbContext().Foodstuffs
        |> Seq.where (fun f -> f.name = name.value)
        |> Seq.map toModel
    
    let private add foodstuff =
        let dbModel = toDb foodstuff
        let ctx = createDbContext()
        ctx.Add(dbModel) |> ignore
        ctx.SaveChanges() |> ignore
        foodstuff
    
    let getDao () = {
        getByIds = getByIds
        search = search
        add = add
    }
    
    