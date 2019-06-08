namespace SmartRecipes.UseCases
open FSharpPlus.Data

module Environment =
    open System
    open SmartRecipes.DataAccess.Tokens
    open SmartRecipes.DataAccess.Users
    open SmartRecipes.DataAccess.Foodstuffs
    open SmartRecipes.DataAccess.ShoppingLists
    open SmartRecipes.DataAccess.Recipes
    
    type IOEnvironment = {
        Users: UsersDao
        Tokens: TokensDao
        ShoppingLists: ShoppingsListsDao
        Foodstuffs: FoodstuffDao
        Recipes: RecipesDao
    }
    
    type Environment = {
        IO: IOEnvironment
        NowUtc: DateTime
    }
    
    module Users =
        let getByEmail email =
            ReaderT(fun env -> env.IO.Users.getByEmail email)
        
        let add account =
            ReaderT(fun env -> env.IO.Users.add account)
            
    module ShoppingList =
        let add shoppingList =
            ReaderT(fun env -> env.IO.ShoppingLists.add shoppingList)