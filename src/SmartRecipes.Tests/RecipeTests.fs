module Tests.Recipes
    open System
    open SmartRecipes
    open Infrastructure
    open Tests
    open Xunit
    open Fake
    
    let getCreateDao () =
        FakeEnvironment(WithToken, WithUser, WithFoodstuff, Map.empty)
    
    let apiIngredientParameter: Api.Recipes.IngredientParameter = {
        foodstuffId = Fake.foodstuff.id.value
        amount = Some {
            value = 10.0
            unit = "liter"
        }
        comment = Some "test"
        displayLine = Some "10 tests"
    }
    
    let apiParameters: Api.Recipes.CreateParameters = {
        Name = Fake.recipe.Name.Value
        PersonCount = 4
        ImageUrl = Fake.recipe.ImageUrl |> Option.map (fun u -> u.AbsolutePath)
        Url = Fake.recipe.Url |> Option.map (fun u -> u.AbsolutePath)
        Description = Some Fake.recipe.Description.Value.Value
        Ingredients = [ apiIngredientParameter ]
        Difficulty = Some "easy"
        CookingTime = Some { Text = "30 minutes" }
        Tags = ["bbq"]
        Rating = Some 8
        Nutrition = {
            Calories = Some 200
            Fat = Some {
                Grams = 100.5
                Percents = Some 50
            }
            SaturatedFat = None
            Sugars = None
            Salt = None
            Protein = None
            Carbs = None
            Fibre = None
        }
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
        Name = ""
        PersonCount = -1
        ImageUrl = Some "not an url"
        Url = Some "not an url"
        Description = Some ""
        Ingredients = [ apiIncorrectIngredientParameter ]
        Difficulty = Some "unknown"
        CookingTime = Some { Text = "" }
        Tags = [""]
        Rating = Some 15
        Nutrition = {
            Calories = Some -10
            Fat = Some {
                Grams = -100.0
                Percents = Some -50
            }
            SaturatedFat = None
            Sugars = None
            Salt = None
            Protein = None
            Carbs = None
            Fibre = None
        }
    }
    
    [<Fact>]
    let ``Can create recipe`` () =
        Api.Recipes.create Fake.token.value apiParameters
        |> ReaderT.execute (getCreateDao ())
        |> Tests.Assert.IsOk
        
    [<Fact>]
    let ``Cannot add recipe with incorrect parameters`` () =
        Api.Recipes.create Fake.token.value apiIncorrectParameters
        |> ReaderT.execute (getCreateDao ())
        |> Assert.IsErrorAnd (function
            |  Api.Recipes.CreateError.ParameterErrors e ->
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.NameCannotBeEmpty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.DescriptionIsProvidedButEmpty e)
                Assert.True (Seq.contains (Api.Recipes.CreateParameterError.AmountError Api.AmountParameters.Error.UnitCannotBeEmpty) e)
                Assert.True (Seq.contains (Api.Recipes.CreateParameterError.AmountError Api.AmountParameters.Error.ValueCannotBeNegative) e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.PersonCountMustBePositive e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.DisplayLineOfIngredientIsProvidedButEmpty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.CommentOfIngredientIsProvidedButEmpty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.UnknownDifficulty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.CookingTimeTextIsProvidedButEmpty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.TagIsEmpty e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.InvalidRating e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.CaloriesMustBePositive e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.GramsMustBePositive e)
                Assert.True (Seq.contains Api.Recipes.CreateParameterError.PercentsMustBePositive e)
                Assert.True (Seq.contains (Api.Recipes.CreateParameterError.InvalidImageUrl "Invalid URI: The format of the URI could not be determined.") e)
                Assert.True (Seq.contains (Api.Recipes.CreateParameterError.InvalidUrl "Invalid URI: The format of the URI could not be determined.") e)
                Assert.Equal (16, (Seq.length e))
            | _ -> Assert.fail("Expected ParameterErrors, got other.")
        )