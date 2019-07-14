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
    
    let apiParameters: Api.Foodstuffs.Create.Parameters = {
        name = Fake.foodstuff.name.Value
        baseAmount = Some {
            unit = "gram"
            value = 1.0
        }
        amountStep = Some 1.0
    }
    
    let apiIncorrectParameters: Api.Foodstuffs.Create.Parameters = {
        name = ""
        baseAmount = Some {
            unit = ""
            value = -1.0
        }
        amountStep = Some -1.0
    }
    
    [<Fact>]
    let ``Can add foodstuff`` () =
        Api.Foodstuffs.Create.create Fake.token.value apiParameters
        |> ReaderT.execute (getFakeCreateDao WithoutFoodstuff)
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff with incorrect parameters`` () =
        Api.Foodstuffs.Create.create  Fake.token.value apiIncorrectParameters
        |> ReaderT.execute (getFakeCreateDao WithoutFoodstuff)
        |> Assert.IsErrorAnd (function
            | Api.Foodstuffs.Create.ParameterErrors es -> 
                Assert.True (Seq.contains Api.Foodstuffs.Create.ParameterError.NameCannotBeEmpty es)
                Assert.True (Seq.contains (Api.Foodstuffs.Create.ParameterError.BaseAmountError Api.AmountParameters.Error.UnitCannotBeEmpty) es)
                Assert.True (Seq.contains (Api.Foodstuffs.Create.ParameterError.BaseAmountError Api.AmountParameters.Error.ValueCannotBeNegative) es)
                Assert.True (Seq.contains Api.Foodstuffs.Create.ParameterError.AmountStepCannotBeNegative es)
            | _ -> Assert.fail("Expecter ParameterErrors, but got other.")
        )