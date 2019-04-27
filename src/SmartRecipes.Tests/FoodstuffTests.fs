module Tests.Foodstuffs
    open Infrastructure
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes
    open SmartRecipes.UseCases
    open SmartRecipes.UseCases.Foodstuffs
    open Tests
    open Xunit
    
    // Use case tests
    
    let getFakeCreateDao withFoodstuff = Fake.environment true false withFoodstuff Map.empty
    
    let parameters = {
        name = Fake.foodstuff.name
        baseAmount = None
        amountStep = None
    }
    
    [<Fact>]
    let ``Can add foodstuff with minimal parameters`` () =
        Foodstuffs.create Fake.token.value parameters
        |> ReaderT.execute (getFakeCreateDao false)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff if already exists`` () =
        Foodstuffs.create Fake.token.value parameters
        |> ReaderT.execute (getFakeCreateDao true)
        |> Tests.Assert.IsError
        
        
    // API tests
    
    let apiParameters: Api.Foodstuffs.CreateParameters = {
        name = Fake.foodstuff.name.value
        baseAmount = {
            unit = "gram"
            value = 1.0
        }
        amountStep = {
            unit = "gram"
            value = 1.0
        }
    }
    
    let apiIncorrectParameters: Api.Foodstuffs.CreateParameters = {
        name = ""
        baseAmount = {
            unit = "unknownUnit"
            value = -1.0
        }
        amountStep = {
            unit = "gram"
            value = 1.0
        }
    }
    
    [<Fact>]
    let ``Can add foodstuff`` () =
        Api.Foodstuffs.create Fake.token.value apiParameters
        |> ReaderT.execute (getFakeCreateDao false)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff with incorrect parameters`` () =
        Api.Foodstuffs.create Fake.token.value apiIncorrectParameters
        |> ReaderT.execute (getFakeCreateDao false)
        |> Assert.IsErrorAnd (fun e -> 
            Assert.True (Seq.contains Api.Foodstuffs.CreateError.NameCannotBeEmpty e)
            Assert.True (Seq.contains Api.Foodstuffs.CreateError.UnknownAmountUnit e)
            Assert.True (Seq.contains Api.Foodstuffs.CreateError.AmountCannotBeNegative e)
        )