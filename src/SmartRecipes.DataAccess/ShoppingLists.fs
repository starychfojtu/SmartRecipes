module DataAccess.ShoppingLists
    open DataAccess
    open Domain.Account
    open Domain.ShoppingList
    open System
    
    type ShoppingsListsDao = {
        add: ShoppingList -> ShoppingList
        get: Account -> ShoppingList
        update: ShoppingList -> ShoppingList
    }
    
    let getDao () = {
        add = fun s -> s
        get = fun a -> raise (NotImplementedException())
        update = fun s -> s
    }