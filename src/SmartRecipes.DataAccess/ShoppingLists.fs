namespace SmartRecipes.DataAccess

open FSharp.Json
open FSharpPlus.Data
open Npgsql.FSharp

module ShoppingLists =
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.ShoppingList
    open MongoDB.Driver

    let private listItemToModel (dbListItem: DbListItem): ListItem = {
        foodstuffId = FoodstuffId(dbListItem.foodstuffId)
        amount = NonNegativeFloat.create dbListItem.amount |> Option.get
    }

    let private listItemToDb (listItem: ListItem): DbListItem = {
        foodstuffId = listItem.foodstuffId.value
        amount = NonNegativeFloat.value listItem.amount
    }

    let private recipeListItemToModel (dbRecipeListItem: DbRecipeListItem): RecipeListItem = {
        recipeId = RecipeId(dbRecipeListItem.recipeId)
        personCount = NaturalNumber.create dbRecipeListItem.personCount |> Option.get
    }

    let private recipeListItemToDb (recipeListItem: RecipeListItem): DbRecipeListItem = {
        recipeId = recipeListItem.recipeId.value
        personCount = int recipeListItem.personCount.Value
    }

    module Postgres =
        let readShoppingList (read: RowReader): ShoppingList = {
            id = ShoppingListId <| read.uuid "id"
            accountId = AccountId <| read.uuid "accountid"
            items = read.string "items" |> Json.deserialize<DbListItem list> |> List.map listItemToModel |> List.map (fun i -> (i.foodstuffId, i)) |> Map.ofList
            recipes = read.string "recipes" |> Json.deserialize<DbRecipeListItem list> |> List.map recipeListItemToModel |> List.map (fun i -> (i.recipeId, i)) |> Map.ofList
        }

        let add conn shoppingList =
            conn
            |> Sql.query "INSERT INTO dbo.shoppinglist VALUES (@id, @accountid, @items, @recipes)"
            |> Sql.parameters [
                "id", Sql.uuid shoppingList.id.value
                "accountid", Sql.uuid shoppingList.accountId.value
                "items", (shoppingList.items |> Map.toList |> List.map (fun (k, v) -> listItemToDb v) |> Json.serialize |> Sql.jsonb)
                "recipes", (shoppingList.recipes |> Map.toList |> List.map (fun (k, v) -> recipeListItemToDb v) |> Json.serialize |> Sql.jsonb)
            ]
            |> Sql.executeNonQuery
            |> fun _ -> shoppingList

        let getByAccount conn (AccountId accountId) =
            conn
            |> Sql.query "SELECT * From dbo.shoppinglist WHERE accountid = @accountid"
            |> Sql.parameters [ "accountid", Sql.uuid accountId ]
            |> Sql.execute readShoppingList
            |> function | Ok l -> List.head l | Error e -> failwith e.Message

        let update conn shoppingList =
            conn
            |> Sql.query "UPDATE dbo.shoppinglist SET items = @items, recipes = @recipes WHERE id = @id"
            |> Sql.parameters [
                "id", Sql.uuid shoppingList.id.value
                "items", (shoppingList.items |> Map.toList |> List.map (fun (k, v) -> listItemToDb v) |> Json.serialize |> Sql.jsonb)
                "recipes", (shoppingList.recipes |> Map.toList |> List.map (fun (k, v) -> recipeListItemToDb v) |> Json.serialize |> Sql.jsonb)
            ]
            |> Sql.executeNonQuery
            |> fun _ -> shoppingList

    module Mongo =

        let private collection = Mongo.getCollection<DbShoppingList> ()

        let private toModel (dbShoppingList: DbShoppingList): ShoppingList = {
            id = ShoppingListId dbShoppingList.id
            accountId = AccountId dbShoppingList.accountId
            items = Seq.map listItemToModel dbShoppingList.items |> Seq.map (fun i -> (i.foodstuffId, i)) |> Map.ofSeq
            recipes = Seq.map recipeListItemToModel dbShoppingList.recipes |> Seq.map (fun i -> (i.recipeId, i)) |> Map.ofSeq
        }

        let private toDb (shoppingList: ShoppingList): DbShoppingList = {
            id = shoppingList.id.value
            accountId = shoppingList.accountId.value
            items = Map.toSeq shoppingList.items |> Seq.map (fun (k, v) -> listItemToDb v)
            recipes = Map.toSeq shoppingList.recipes |> Seq.map (fun (k, v) -> recipeListItemToDb v)
        }

        let add shoppingList =
            toDb shoppingList |> collection.InsertOne |> ignore
            shoppingList

        let getByAccount (AccountId accountId) =
            collection.AsQueryable()
            |> Seq.filter (fun l -> l.accountId = accountId)
            |> Seq.head
            |> toModel

        let update shoppingList =
            let dbShoppingList = toDb shoppingList
            collection.ReplaceOne((fun i -> i.id = dbShoppingList.id), dbShoppingList) |> ignore
            shoppingList