namespace SmartRecipes.UseCases

open System.Linq
open FSharpPlus
open FSharpPlus.Lens
open SmartRecipes.Domain
open SmartRecipes.IO
open SmartRecipes.Domain.ShoppingList
open FSharpPlus.Data
open SmartRecipes.Domain.Foodstuff
open SmartRecipes.Domain.Foodstuff
open SmartRecipes.IO
open SmartRecipes.Recommendations
open SmartRecipes.Recommendations.Library

module ShoppingLists =
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

    let private getFoodstuffAmounts (foodstuffs: Foodstuff seq) shoppingList = query {
        for f in foodstuffs.AsQueryable() do
        join i in shoppingList.items on (f.id = i.Key)
        let amount = i.Value.amount.Value * f.baseAmount.value.Value |> NonNegativeFloat.create |> Option.get
        select { FoodstuffId = f.id; Amount = Some { unit = f.baseAmount.unit; value = amount } }
    }

    let private getRecommendedRecipes foodstuffAmounts = monad {
        let! statistics = Recipes.recommendationStatistics |> IO.success
        let! vectors = Foodstuffs.vectors |> IO.success
        return Recommendations.calibratedWord2Vec statistics vectors foodstuffAmounts
    }

    let recommend accessToken = monad {
        let! accountId = Users.authorize Unaturhorized accessToken
        let! shoppingList = ShoppingLists.getByAccount accountId |> IO.toSuccessEIO
        if Map.count shoppingList.items = 0
            then []
            else
                let foodstuffIds = shoppingList.items |> Seq.map (fun kvp -> kvp.Key.value)
                let! foodstuffs = Foodstuffs.getByIds foodstuffIds |> IO.toSuccessEIO
                let foodstuffAmounts = getFoodstuffAmounts foodstuffs shoppingList |> Seq.toList
                let! recommendations = getRecommendedRecipes foodstuffAmounts
                return recommendations
    }