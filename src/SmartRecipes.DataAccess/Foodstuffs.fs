module DataAccess.Foodstuffs
    open System
    open DataAccess.Model
    open FSharpPlus.Data
    open Domain.Foodstuff
    open Domain.Foodstuff
    open Domain.NonEmptyString
    open Domain.NonNegativeFloat
    open Infrastructure.Validation
    open MongoDB.Driver
    
    type FoodstuffDao = {
        getByIds: seq<Guid> -> seq<Foodstuff>
        getById: Guid -> Foodstuff option
        search: NonEmptyString -> seq<Foodstuff>
        add: Foodstuff -> Foodstuff
    }
    
    let private collection () = Database.getCollection<DbFoodstuff> ()
    
    let internal amountToDb amount : DbAmount = {
        unit = amount.unit
        value = amount.value.value
    }
    
    let internal amountToModel (dbAmount: DbAmount) = {
        unit = dbAmount.unit
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
        collection().Find(fun f -> Seq.contains f.id ids).ToEnumerable()
        |> Seq.map toModel
        
    let private getById id =
        collection().Find(fun f -> f.id = id).ToEnumerable()
        |> Seq.map toModel
        |> Seq.tryHead
    
    let private search (name: NonEmptyString) =
        collection().Find(fun f -> f.name = name.value).ToEnumerable()
        |> Seq.map toModel
    
    let private add foodstuff =
        toDb foodstuff |> collection().InsertOne |> ignore
        foodstuff
    
    let getDao () = {
        getByIds = getByIds
        getById = getById
        search = search
        add = add
    }
    
    