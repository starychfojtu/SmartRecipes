module Tests.ShoppingLists
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes
    open SmartRecipes.Api.ShoppingLists
    open SmartRecipes.Domain
    open System
    open Tests
    open SmartRecipes.UseCases.ShoppingLists
    open Xunit

    let getFakeShoppingListActionDao withFoodstuff items =
        Fake.environment true true withFoodstuff items

    // Add foodstuff
    
    let addFoodstuffsParameters = {
        itemIds = seq { yield Fake.foodstuff.id.value }
    }
    
    let fakeItems = Map.add Fake.listItem.foodstuffId Fake.listItem Map.empty
    
    [<Fact>]
    let ``Can add foodstuffs`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> ReaderT.execute (getFakeShoppingListActionDao true Map.empty)
        |> Assert.IsOk
        
    [<Fact>]
    let ``Cannot add not existing foodstuff`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> ReaderT.execute (getFakeShoppingListActionDao false Map.empty)
        |> Assert.IsErrorAnd (fun e -> Assert.Equal(e, Api.ShoppingLists.AddItemsError.InvalidIds))
        
    [<Fact>]
    let ``Cannot add already added foodstuff`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> ReaderT.execute (getFakeShoppingListActionDao true fakeItems)
        |> Assert.IsError
        
    // Change foodstuff amount
        
    let changeAmountParameters = {
        foodstuffId = Fake.listItem.foodstuffId.value
        amount = NonNegativeFloat.value Fake.listItem.amount
    }
    
    let changeAmountIncorrectParameters = {
        foodstuffId = Guid.NewGuid ()
        amount = -1.0
    }
    
    [<Fact>]
    let ``Can change amount`` () =
        Api.ShoppingLists.changeAmount Fake.accessToken.value.value changeAmountParameters
        |> ReaderT.execute (getFakeShoppingListActionDao true fakeItems)
        |> Assert.IsOk
        
    [<Fact>]
    let ``Cannot change foodstuff on shit shit shit`` () =
        Api.ShoppingLists.changeAmount Fake.accessToken.value.value changeAmountIncorrectParameters
        |> ReaderT.execute (getFakeShoppingListActionDao false Map.empty)
        |> Assert.IsError
        
    // Cook recipe
    
    // TODO: Tests for cooking of recipe