namespace SmartRecipes.DataAccess

module Foodstuffs =
    open System
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open MongoDB.Driver
    open Utils

    type FoodstuffDao = {
        getByIds: seq<Guid> -> seq<Foodstuff>
        getById: Guid -> Foodstuff option
        search: NonEmptyString -> seq<Foodstuff>
        add: Foodstuff -> Foodstuff
    }
    
    let private collection = Database.getCollection<DbFoodstuff> ()
                
    let internal toDb foodstuff : DbFoodstuff = {
        id = match foodstuff.id with FoodstuffId id -> id
        name = foodstuff.name.Value
        baseAmount = amountToDb foodstuff.baseAmount
        amountStep = NonNegativeFloat.value foodstuff.amountStep
    }
    
    
    let internal toModel (dbFoodstuff: DbFoodstuff) = {
        id = FoodstuffId dbFoodstuff.id 
        name = create dbFoodstuff.name |> Option.get
        baseAmount = amountToModel dbFoodstuff.baseAmount
        amountStep = NonNegativeFloat.create dbFoodstuff.amountStep |> Option.get
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
        |> Seq.filter (fun f -> f.name = name.Value)
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
    
    