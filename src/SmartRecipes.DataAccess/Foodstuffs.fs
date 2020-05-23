namespace SmartRecipes.DataAccess

open MongoDB.Bson
open Users
open FSharpPlus.Data
open Model
open SmartRecipes.Domain
open SmartRecipes.Domain.Foodstuff
open MongoDB.Driver
open Utils
open SearchQuery
open Npgsql.FSharp

module Foodstuffs =
    module Postgres =
        let readFoodstuff (read: RowReader): Foodstuff = {
            id = FoodstuffId <| read.uuid "id"
            name = toNonEmptyStringModel <| read.string "name"
            baseAmount = {
                unit = MetricUnit.MetricUnit <| toNonEmptyStringModel "pieces"
                value = toNonNegativeFloatModel 1.0
            }
            amountStep = toNonNegativeFloatModel 1.0
        }

        let getByIds conn ids =
            conn
            |> Sql.query "SELECT * From dbo.foodstuff WHERE id IN @ids"
            |> Sql.parameters [ "ids", Sql.uuidArray <| Seq.toArray ids ]
            |> Sql.execute readFoodstuff
            |> function | Ok l -> l | Error e -> failwith e.Message

        let search conn (query: SearchQuery) =
            conn
            |> Sql.query "SELECT * From dbo.foodstuff WHERE name LIKE '%@query%'"
            |> Sql.parameters [ "query", Sql.string query.Value ]
            |> Sql.execute readFoodstuff
            |> function | Ok l -> l | Error e -> failwith e.Message

    module Mongo =

        let private collection = Mongo.getCollection<DbFoodstuff> ()

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

