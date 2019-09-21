namespace SmartRecipes.DataAccess

open MongoDB.Bson

module Foodstuffs =
    open System
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open MongoDB.Driver
    open Utils
    open SearchQuery
        
    module Mongo = 
    
        let private collection = Database.getCollection<DbFoodstuff> ()
                    
        let internal toDb foodstuff : DbFoodstuff = {
            id = match foodstuff.id with FoodstuffId id -> id
            name = foodstuff.name.Value
            baseAmount = amountToDb foodstuff.baseAmount
            amountStep = NonNegativeFloat.value foodstuff.amountStep
        }
        
        
        let internal toModel (dbFoodstuff: DbFoodstuff) = {
            id = FoodstuffId dbFoodstuff.id 
            name = NonEmptyString.create dbFoodstuff.name |> Option.get
            baseAmount = amountToModel dbFoodstuff.baseAmount
            amountStep = NonNegativeFloat.create dbFoodstuff.amountStep |> Option.get
        }
        
        let getByIds ids =
            collection.AsQueryable()
            |> Seq.filter (fun f -> Seq.contains f.id ids)
            |> Seq.map toModel
    
        let search (query: SearchQuery) =
            let regex = BsonRegularExpression(query.Value)
            let filter = Builders<DbFoodstuff>.Filter.Regex((fun f -> f.name :> obj), regex)
            collection.FindSync(filter).ToEnumerable() |> Seq.map toModel
        
        let add foodstuff =
            toDb foodstuff |> collection.InsertOne |> ignore
            foodstuff
    
    