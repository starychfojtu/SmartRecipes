namespace SmartRecipes.IO

open FSharpPlus.Data
open SmartRecipes.Domain.Account
open SmartRecipes.Domain.ShoppingList

module ShoppingLists =
    
    type IShoppingsListsDao = 
        abstract member getByAccount: AccountId -> ShoppingList
        abstract member update: ShoppingList -> ShoppingList
        abstract member add: ShoppingList -> ShoppingList
        
    let getByAccount<'e when 'e :> IShoppingsListsDao> id = Reader(fun (shoppingLists : 'e) -> shoppingLists.getByAccount id)
    let update<'e when 'e :> IShoppingsListsDao> shoppingList = Reader(fun (shoppingLists : 'e) -> shoppingLists.update shoppingList)
    let add<'e when 'e :> IShoppingsListsDao> shoppingList = Reader(fun (shoppingLists : 'e) -> shoppingLists.add shoppingList)