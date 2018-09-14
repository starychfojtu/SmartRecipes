module Tests.ShoppingLists
    open FSharpPlus.Data
    open Infrastructure
    open Api.ShoppingLists
    open Tests
    open UseCases.ShoppingLists
    open Xunit

    let addFoodstuffsParameters = {
        itemIds = seq { yield Fake.foodstuff.id.value }
    }
    
    let getFakeAddItemsDao items: AddItemDao = {
        tokens = Fake.tokensDao true
        shoppingLists = Fake.shoppingListDao items
    }
    
    let getAddFoodstuffsDao withFoodstuff items = (getFakeAddItemsDao items, (Fake.foodstuffsDao withFoodstuff).getByIds)
    
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