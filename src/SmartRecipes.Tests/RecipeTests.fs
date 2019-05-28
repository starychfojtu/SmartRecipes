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
    
    let apiParameters: Api.Recipes.CreateParameters = {
        Name = Fake.recipe.Name.Value
        PersonCount = 4
        ImageUrl = Fake.recipe.ImageUrl |> Option.map (fun u -> u.AbsolutePath)
        Url = Fake.recipe.Url |> Option.map (fun u -> u.AbsolutePath)
        Description = Some Fake.recipe.Description.Value.Value
        Ingredients = seq { yield apiIngredientParameter }
        Difficulty = Some "easy"
        CookingTime = Some { Text = "30 minutes" }
        Tags = ["bbq"]
        Rating = Some 8
        Nutrition = {
            Calories = Some 200
            Fat = Some {
                Grams = 100
                Percents = Some 50
            }
            SaturatedFat = None
            Sugars = None
            Protein = None
            Carbs = None
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
        Ingredients = seq { yield apiIncorrectIngredientParameter }
        Difficulty = Some "unknown"
        CookingTime = Some { Text = "" }
        Tags = [""]
        Rating = Some 15
        Nutrition = {
            Calories = Some -10
            Fat = Some {
                Grams = -100
                Percents = Some -50
            }
            SaturatedFat = None
            Sugars = None
            Protein = None
            Carbs = None
        }
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
            Assert.True (Seq.contains Api.Recipes.CreateError.UnknownDifficulty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.CookingTimeTextIsProvidedButEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.TagIsEmpty e)
            Assert.True (Seq.contains Api.Recipes.CreateError.InvalidRating e)
            Assert.True (Seq.contains Api.Recipes.CreateError.CaloriesMustBePositive e)
            Assert.True (Seq.contains Api.Recipes.CreateError.GramsMustBePositive e)
            Assert.True (Seq.contains Api.Recipes.CreateError.PercentsMustBePositive e)
            Assert.Equal (15, (Seq.length e))
        )