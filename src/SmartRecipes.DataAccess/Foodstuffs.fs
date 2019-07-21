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
    
    type IFoodstuffDao = 
        abstract member getByIds: seq<Guid> -> seq<Foodstuff>
        abstract member search: SearchQuery -> seq<Foodstuff>
        abstract member add: Foodstuff -> Foodstuff
            
    let getByIds<'e when 'e :> IFoodstuffDao> ids = Reader(fun (e : 'e) -> e.getByIds ids)
    let search<'e when 'e :> IFoodstuffDao> query = Reader(fun (e : 'e) -> e.search query)
    let add<'e when 'e :> IFoodstuffDao> foodstuff = Reader(fun (e : 'e) -> e.add foodstuff)
        
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
    
    