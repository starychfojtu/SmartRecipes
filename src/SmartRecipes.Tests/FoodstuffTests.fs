module Tests.Foodstuffs
    open Infrastructure
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes
    open SmartRecipes.UseCases
    open SmartRecipes.UseCases.Foodstuffs
    open Tests
    open Xunit
    open Fake
    
    // Use case tests
    
    let getFakeCreateDao foodstuffOptions =
        Fake.environment WithToken WithUser foodstuffOptions Map.empty
    
    let parameters = {
        name = Fake.foodstuff.name
        baseAmount = None
        amountStep = None
    }
    
    [<Fact>]
    let ``Can add foodstuff with minimal parameters`` () =
        Foodstuffs.create Fake.token.value parameters
        |> ReaderT.execute (getFakeCreateDao WithoutFoodstuff)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff if already exists`` () =
        Foodstuffs.create Fake.token.value parameters
        |> ReaderT.execute (getFakeCreateDao WithFoodstuff)
        |> Tests.Assert.IsError
        
        
    // API tests
    
    let apiParameters: Api.Foodstuffs.CreateParameters = {
        name = Fake.foodstuff.name.value
        baseAmount = {
            unit = "gram"
            value = 1.0
        }
        amountStep = 1.0
    }
    
    let apiIncorrectParameters: Api.Foodstuffs.CreateParameters = {
        name = ""
        baseAmount = {
            unit = "unknownUnit"
            value = -1.0
        }
        amountStep = -1.0
    }
    
    [<Fact>]
    let ``Can add foodstuff`` () =
        Api.Foodstuffs.create Fake.token.value apiParameters
        |> ReaderT.execute (getFakeCreateDao WithoutFoodstuff)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff with incorrect parameters`` () =
        Api.Foodstuffs.create Fake.token.value apiIncorrectParameters
        |> ReaderT.execute (getFakeCreateDao WithoutFoodstuff)
        |> Assert.IsErrorAnd (fun e -> 
            Assert.True (Seq.contains Api.Foodstuffs.CreateError.NameCannotBeEmpty e)
            Assert.True (Seq.contains (Api.Foodstuffs.CreateError.BaseAmountError Api.Foodstuffs.ParseAmountError.UnknownUnit) e)
            Assert.True (Seq.contains (Api.Foodstuffs.CreateError.BaseAmountError Api.Foodstuffs.ParseAmountError.ValueCannotBeNegative) e)
            Assert.True (Seq.contains Api.Foodstuffs.CreateError.AmountStepCannotBeNegative e)
        )