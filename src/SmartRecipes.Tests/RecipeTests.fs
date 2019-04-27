module Tests.Recipes
    open System
    open SmartRecipes
    open Infrastructure
    open Tests
    open Xunit
    
    let getCreateDao () = Fake.environment true true true Map.empty
    
    let apiIngredientParameter: Api.Recipes.IngredientParameter = {
        foodstuffId = Fake.foodstuff.id.value
        amount = 10.0
    }
    
    let apiParameters: Api.Recipes.CreateParameters = {
        name = Fake.recipe.name.value
        personCount = 4
        imageUrl = Fake.recipe.imageUrl.AbsolutePath
        description = Fake.recipe.description.Value.value
        ingredients = seq { yield apiIngredientParameter }
    }
    
    let apiIncorrectIngredientParameter: Api.Recipes.IngredientParameter = {
        foodstuffId = Guid.NewGuid()
        amount = -10.0
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
        |> Reader.execute (getCreateDao ())
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add foodstuff with incorrect parameters`` () =
        Api.Recipes.create Fake.token.value apiIncorrectParameters
        |> Reader.execute (getCreateDao ())
        |> Assert.IsErrorAnd (fun e -> 
            Assert.True (Seq.contains Api.Recipes.CreateError.NameCannotBeEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.DescriptionIsProvidedButEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.AmountOfIngredientMustBePositive e)
            Assert.True (Seq.contains Api.Recipes.CreateError.PersonCountMustBePositive e)
            Assert.Equal ((Seq.length e), 5)
        )