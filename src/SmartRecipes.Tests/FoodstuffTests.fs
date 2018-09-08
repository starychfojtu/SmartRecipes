module Tests.Foodstuffs
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Tokens
    open Infrastructure
    open Domain
    open Domain.Account
    open Domain.Token
    open Domain.Foodstuff
    open UseCases
    open UseCases.Foodstuffs
    open Utils
    open NonEmptyString
    open NonNegativeFloat
    open System
    open Xunit
    
    let token = Token("fake")
    let accessToken: AccessToken = {
        accountId = AccountId(Guid.Empty)
        value = token
        expiration = DateTime.UtcNow.AddDays(1.0)
    }
    let foodstuff: Foodstuff = {
        id = FoodstuffId(Guid.Empty)
        name = mkNonEmptyString "Test" |> forceSucces
        baseAmount = {
            unit = Gram
            value = mkNonNegativeFloat 100.0 |> forceSucces
        }
        amountStep = {
            unit = Gram
            value = mkNonNegativeFloat 10.0 |> forceSucces
        }
    }
    
    let getFakeTokensDao (): TokensDao = {
        get = fun v -> Some accessToken
        add = fun f -> f
    }
    
    let getFakeFoodstuffsDao withFoodstuff : FoodstuffDao = {
        getByIds = if withFoodstuff then fun ids -> seq { yield foodstuff } else fun ids -> Seq.empty
        search = if withFoodstuff then fun name -> seq { yield foodstuff } else fun name -> Seq.empty
        add = fun f -> f
    }
    
    let getFakeCreateDao withFoodstuff: CreateFoodstuffDao = {
        tokens = getFakeTokensDao ()
        foodstuffs = getFakeFoodstuffsDao withFoodstuff
    }
    
    [<Fact>]
    let ``Can add foodstuff with minimal parameters`` () =
        let parameters = {
            name = foodstuff.name
            baseAmount = None
            amountStep = None
        }
        Foodstuffs.create token.value parameters
        |> Reader.execute (getFakeCreateDao false)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff if already exists`` () =
        let parameters = {
            name = foodstuff.name
            baseAmount = None
            amountStep = None
        }
        Foodstuffs.create token.value parameters
        |> Reader.execute (getFakeCreateDao true)
        |> Tests.Assert.IsError