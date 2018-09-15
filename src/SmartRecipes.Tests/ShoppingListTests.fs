module Tests.ShoppingLists
    open FSharpPlus.Data
    open Infrastructure
    open Api.ShoppingLists
    open System
    open Tests
    open UseCases.ShoppingLists
    open Xunit

    let getFakeShoppingListActionDao items: ShoppingListActionDao = {
        tokens = Fake.tokensDao true
        shoppingLists = Fake.shoppingListDao items
    }

    // Add foodstuff
    
    let addFoodstuffsParameters = {
        itemIds = seq { yield Fake.foodstuff.id.value }
    }
    
    let getAddFoodstuffsDao withFoodstuff items = (getFakeShoppingListActionDao items, (Fake.foodstuffsDao withFoodstuff).getByIds)
    
    let fakeItems = Map.add Fake.listItem.foodstuffId Fake.listItem Map.empty
    
    [<Fact>]
    let ``Can add foodstuffs`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> Reader.execute (getAddFoodstuffsDao true Map.empty)
        |> Assert.IsOk
        
    [<Fact>]
    let ``Cannot add not existing foodstuff`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> Reader.execute (getAddFoodstuffsDao false Map.empty)
        |> Assert.IsErrorAnd (fun e -> Assert.Equal(e, Api.ShoppingLists.AddItemsError.InvalidIds))
        
    [<Fact>]
    let ``Cannot add already added foodstuff`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> Reader.execute (getAddFoodstuffsDao true fakeItems)
        |> Assert.IsErrorAnd (fun e ->
            Assert.Equal(e,  Api.ShoppingLists.AddItemsError.BusinessError(UseCases.ShoppingLists.AddItemError.FoodstuffAlreadyAdded))
        )
        
    // Change foodstuff amount
        
    let changeAmountParameters = {
        foodstuffId = Fake.listItem.foodstuffId.value
        amount = Fake.listItem.amount.value
    }
    
    let changeAmountIncorrectParameters = {
        foodstuffId = Guid.NewGuid ()
        amount = -1.0
    }
    
    let getChangeAmountDao withFoodstuff items = {
        shoppingListAction = getFakeShoppingListActionDao items
        foodstuffs = Fake.foodstuffsDao withFoodstuff
    }
    
    [<Fact>]
    let ``Can change amount`` () =
        Api.ShoppingLists.changeAmount Fake.accessToken.value.value changeAmountParameters
        |> Reader.execute (getChangeAmountDao true fakeItems)
        |> Assert.IsOk
        
    [<Fact>]
    let ``Cannot change foodstuff on shit shit shit`` () =
        Api.ShoppingLists.changeAmount Fake.accessToken.value.value changeAmountIncorrectParameters
        |> Reader.execute (getChangeAmountDao false Map.empty)
        |> Assert.IsError