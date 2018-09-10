module DataAccess.ShoppingLists
    open DataAccess
    open Domain.ShoppingList
    
    type ShoppingListDao = {
        add: ShoppingList -> ShoppingList
    }
    
    let getDao () = {
        add = fun s -> s
    }