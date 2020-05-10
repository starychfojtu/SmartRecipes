namespace SmartRecipes.DataAccess
open FSharpPlus.Data

module ShoppingLists =
    open Model
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.ShoppingList
    open MongoDB.Driver

    module Mongo =
        
        let private collection = Mongo.getCollection<DbShoppingList> ()
        
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