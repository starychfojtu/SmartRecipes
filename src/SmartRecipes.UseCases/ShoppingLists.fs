namespace SmartRecipes.UseCases

module ShoppingLists =
    open SmartRecipes.Domain
    open SmartRecipes.Recommendations
    open SmartRecipes.DataAccess
    open SmartRecipes.Domain.ShoppingList
    open FSharpPlus.Data
    open Infrastructure
    
    let private getShoppingList accountId =
        ShoppingLists.getByAccount accountId |> ReaderT.hoistOk
        
    let private updateShoppingList list = 
        ShoppingLists.update list |> ReaderT.hoistOk
    
    let private shoppingListAction accessToken authorizeError action =
        Users.authorize authorizeError accessToken
        >>= getShoppingList
        >>= action
        >>= updateShoppingList
        
    // Get
        
    type GetShoppingListError =
        | Unauthorized
    
    let get accessToken =
        Users.authorize Unauthorized accessToken
        >>= getShoppingList
        
    // Add foodstuff
    
    type AddItemsError =
        | Unauthorized
        | DomainError of ShoppingList.AddItemError
        
    let private addFoodstuffsToList foodstuffs list = 
        ShoppingList.addFoodstuffs foodstuffs list |> Result.mapError DomainError |> ReaderT.id
        
    let addFoodstuffs accesToken foodstuffs = 
        shoppingListAction accesToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes recipes list |> Result.mapError DomainError |> ReaderT.id
        
    let addRecipes accessToken recipes = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipes)

    // Change amounts
    
    type ChangeAmountError =
        | Unauthorized
        | DomainError of ShoppingList.ChangeAmountError
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        ShoppingList.changeAmount foodstuff newAmount list |> Result.mapError DomainError |> ReaderT.id
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        ShoppingList.changePersonCount recipe newPersonCount list |> Result.mapError DomainError |> ReaderT.id
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeRecipePersonCount recipe newPersonCount)
        
    // Remove foodstuff 
    
    type RemoveItemsError =
        | Unauthorized
        | DomainError of ShoppingList.RemoveItemError
    
    let private removeFoodstuffsFromList foodstuffIds list =
        removeFoodstuffs foodstuffIds list |> Result.mapError DomainError |> ReaderT.id
    
    let removeFoodstuffs accessToken foodstuffIds = 
        shoppingListAction accessToken Unauthorized (removeFoodstuffsFromList foodstuffIds)
        
    // Remove recipe
    
    let private removeRecipesFromList recipeIds list =
        removeRecipes recipeIds list |> Result.mapError DomainError |> ReaderT.id
    
    let removeRecipes accessToken recipeIds = 
        shoppingListAction accessToken Unauthorized (removeRecipesFromList recipeIds)
        
    // Recommend
    
    type RecommendError =
        | Unaturhorized
        
    let private getRecommendedRecipes (shoppingList: ShoppingList) =
        let foodstuffIds = shoppingList.items |> Map.toSeq |> Seq.map fst |> Seq.toList
        let sort = Recommendations.sort { Input.FoodstuffIds = foodstuffIds }
        Recipes.getRecommendedationCandidates foodstuffIds
        |> Reader.map sort
        |> ReaderT.hoistOk
    
    let recommend accessToken =
        Users.authorize Unaturhorized accessToken
        >>= getShoppingList
        >>= getRecommendedRecipes