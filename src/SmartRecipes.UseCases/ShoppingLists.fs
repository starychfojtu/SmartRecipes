namespace SmartRecipes.UseCases

module ShoppingLists =
    open SmartRecipes.Domain
    open SmartRecipes.Recommendations
    open SmartRecipes.IO
    open SmartRecipes.Domain.ShoppingList
    open FSharpPlus.Data
    open Infrastructure
    
    let private shoppingListAction accessToken authorizeError action =
        Users.authorize authorizeError accessToken
        >>= (ShoppingLists.getByAccount >> IO.toSuccessEIO)
        >>= action
        >>= (ShoppingLists.update >> IO.toSuccessEIO)
        
    // Get
        
    type GetShoppingListError =
        | Unauthorized
    
    let get accessToken =
        Users.authorize Unauthorized accessToken
        >>= (ShoppingLists.getByAccount >> IO.toSuccessEIO)
        
    // Add foodstuff
    
    type AddItemsError =
        | Unauthorized
        | DomainError of ShoppingList.AddItemError
        
    let private addFoodstuffsToList foodstuffs list = 
        ShoppingList.addFoodstuffs foodstuffs list
        |> Result.mapError DomainError
        |> IO.fromResult
        
    let addFoodstuffs accessToken foodstuffs = 
        shoppingListAction accessToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes recipes list
        |> Result.mapError DomainError
        |> IO.fromResult
        
    let addRecipes accessToken recipes = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipes)

    // Change amounts
    
    type ChangeAmountError =
        | Unauthorized
        | DomainError of ShoppingList.ChangeAmountError
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        ShoppingList.changeAmount foodstuff newAmount list
        |> Result.mapError DomainError
        |> IO.fromResult
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        ShoppingList.changePersonCount recipe newPersonCount list
        |> Result.mapError DomainError
        |> IO.fromResult
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeRecipePersonCount recipe newPersonCount)
        
    // Remove foodstuff 
    
    type RemoveItemsError =
        | Unauthorized
        | DomainError of ShoppingList.RemoveItemError
    
    let private removeFoodstuffsFromList foodstuffIds list =
        removeFoodstuffs foodstuffIds list
        |> Result.mapError DomainError
        |> IO.fromResult
    
    let removeFoodstuffs accessToken foodstuffIds = 
        shoppingListAction accessToken Unauthorized (removeFoodstuffsFromList foodstuffIds)
        
    // Remove recipe
    
    let private removeRecipesFromList recipeIds list =
        removeRecipes recipeIds list
        |> Result.mapError DomainError
        |> IO.fromResult
    
    let removeRecipes accessToken recipeIds = 
        shoppingListAction accessToken Unauthorized (removeRecipesFromList recipeIds)
        
    // Recommend
    
    type RecommendError =
        | Unaturhorized
        
    let private getRecommendedRecipes (shoppingList: ShoppingList) =
        let foodstuffIds = shoppingList.items |> Map.toSeq |> Seq.map fst |> Seq.toList
        let sort = Recommendations.sort { Input.FoodstuffIds = foodstuffIds }
        Recipes.getRecommendationCandidates foodstuffIds
        |> Reader.map (sort >> Seq.truncate 10)
        |> ReaderT.hoistOk
    
    let recommend accessToken =
        Users.authorize Unaturhorized accessToken
        >>= (ShoppingLists.getByAccount >> IO.toSuccessEIO)
        >>= getRecommendedRecipes