namespace SmartRecipes.DataAccess

module Foodstuffs =
    open System
    open FSharpPlus.Data
    open Infrastructure.Validation
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open MongoDB.Driver

    type FoodstuffDao = {
        getByIds: seq<Guid> -> seq<Foodstuff>
        getById: Guid -> Foodstuff option
        search: NonEmptyString -> seq<Foodstuff>
        add: Foodstuff -> Foodstuff
    }
    
    let private collection = Database.getCollection<DbFoodstuff> ()
    
    let internal amountToDb amount : DbAmount = {
        unit = amount.unit
        value = NonNegativeFloat.value amount.value 
    }
    
    let internal amountToModel (dbAmount: DbAmount) = {
        unit = dbAmount.unit
        value = NonNegativeFloat.create dbAmount.value |> forceSucces
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
        collection.AsQueryable()
        |> Seq.filter (fun f -> Seq.contains f.id ids)
        |> Seq.map toModel
        
    let private getById id =
        collection.AsQueryable()
        |> Seq.filter (fun f -> f.id = id)
        |> Seq.map toModel
        |> Seq.tryHead
    
    let private search (name: NonEmptyString) =
        collection.AsQueryable()
        |> Seq.filter (fun f -> f.name = name.value)
        |> Seq.map toModel
    
    let private add foodstuff =
        toDb foodstuff |> collection.InsertOne |> ignore
        foodstuff
    
    let dao = {
        getByIds = getByIds
        getById = getById
        search = search
        add = add
    }
    
    