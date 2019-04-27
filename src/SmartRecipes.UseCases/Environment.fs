namespace SmartRecipes.UseCases

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

