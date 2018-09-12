module Tests.Foodstuffs
    open Infrastructure
    open Domain
    open Domain.Account
    open Domain.Token
    open Domain.Foodstuff
    open UseCases
    open UseCases.Foodstuffs
    open NonEmptyString
    open NonNegativeFloat
    open System
    open Xunit
    
    let getFakeCreateDao withFoodstuff: CreateFoodstuffDao = {
        tokens = Fake.tokensDao true
        foodstuffs = Fake.foodstuffsDao withFoodstuff
    }
    
    let parameters = {
        name = Fake.foodstuff.name
        baseAmount = None
        amountStep = None
    }
    
    [<Fact>]
    let ``Can add foodstuff with minimal parameters`` () =
        Foodstuffs.create Fake.token.value parameters
        |> Reader.execute (getFakeCreateDao false)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff if already exists`` () =
        Foodstuffs.create Fake.token.value parameters
        |> Reader.execute (getFakeCreateDao true)
        |> Tests.Assert.IsError