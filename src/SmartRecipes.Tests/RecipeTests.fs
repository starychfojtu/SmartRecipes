module Tests.Recipes
    open System
    open SmartRecipes
    open Infrastructure
    open Tests
    open Xunit
    open Fake
    
    let getCreateDao () =
        Fake.environment WithToken WithUser WithFoodstuff Map.empty
    
    let apiIngredientParameter: Api.Recipes.IngredientParameter = {
        foodstuffId = Fake.foodstuff.id.value
        amount = Some {
            value = 10.0
            unit = "liter"
        }
        comment = Some "test"
        displayLine = Some "10 tests"
    }
    
    // Dodelat testy, doprepsat shopping list na Parse, podivat se jestli funguje api s parsingem a doprepsat parsing jsonu na konzistentni s optionama
    
    let apiParameters: Api.Recipes.CreateParameters = {
        Name = Fake.recipe.Name.Value
        PersonCount = 4
        ImageUrl = Fake.recipe.ImageUrl.AbsolutePath
        Description = Fake.recipe.Description.Value.Value
        Ingredients = seq { yield apiIngredientParameter }
    }
    
    let apiIncorrectIngredientParameter: Api.Recipes.IngredientParameter = {
        foodstuffId = Guid.NewGuid()
        amount = Some {
            value = -10.0
            unit = ""
        }
        comment = Some ""
        displayLine = Some ""
    }
        
    let apiIncorrectParameters: Api.Recipes.CreateParameters = {
        name = ""
        personCount = -1
        imageUrl = "not an url"
        description = ""
        ingredients = seq { yield apiIncorrectIngredientParameter }
    }
    
    [<Fact>]
    let ``Can create recipe`` () =
        Api.Recipes.create Fake.token.value apiParameters
        |> ReaderT.execute (getCreateDao ())
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff with incorrect parameters`` () =
        Api.Recipes.create Fake.token.value apiIncorrectParameters
        |> ReaderT.execute (getCreateDao ())
        |> Assert.IsErrorAnd (fun e -> 
            Assert.True (Seq.contains Api.Recipes.CreateError.NameCannotBeEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.DescriptionIsProvidedButEmpty e)
            Assert.True (Seq.contains (Api.Recipes.CreateError.AmountError Api.Foodstuffs.ParseAmountError.UnitCannotBeEmpty) e)
            Assert.True (Seq.contains (Api.Recipes.CreateError.AmountError Api.Foodstuffs.ParseAmountError.ValueCannotBeNegative) e)
            Assert.True (Seq.contains Api.Recipes.CreateError.PersonCountMustBePositive e)
            Assert.True (Seq.contains Api.Recipes.CreateError.DisplayLineOfIngredientIsProvidedButEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.CommentOfIngredientIsProvidedButEmpty e)
            Assert.Equal (8, (Seq.length e))
        )