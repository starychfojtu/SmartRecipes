module Tests.ShoppingLists
    open FSharpPlus.Data
    open Infrastructure
    open Api.ShoppingLists
    open Tests
    open Xunit

    let addFoodstuffsParameters = {
        foodstuffIds = seq { yield Fake.foodstuff.id.value }
    }
    
    let getAddFoodstuffsDao withFoodstuff items = {
        tokens = Fake.tokensDao true
        shoppingLists = Fake.shoppingListDao items
        foodstuffs = Fake.foodstuffsDao withFoodstuff
    }
    
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
        |> Assert.IsErrorAnd (fun e -> Assert.Equal(e, FoodstuffNotFound))
        
    [<Fact>]
    let ``Cannot add already added foodstuff`` () =
        Api.ShoppingLists.addFoodstuffs Fake.accessToken.value.value addFoodstuffsParameters
        |> Reader.execute (getAddFoodstuffsDao false fakeItems)
        |> Assert.IsErrorAnd (fun e -> Assert.Equal(e, FoodstuffNotFound))