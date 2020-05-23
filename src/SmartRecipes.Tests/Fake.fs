module Tests.Fake
    open SmartRecipes.IO.Foodstuffs
    open SmartRecipes.IO.Recipes
    open SmartRecipes.Domain
    open SmartRecipes.UseCases.DateTimeProvider
    open Infrastructure
    open System
    open System.Net.Mail
    open SmartRecipes.Domain.Account
    open SmartRecipes.IO.Users
    open SmartRecipes.IO.Tokens
    open SmartRecipes.IO.ShoppingLists
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.ShoppingList
    open SmartRecipes.Domain.Token
    open FSharpPlus.Data
    open SmartRecipes.Domain

    // Data

    let id = Guid.NewGuid()
    let mailAddress = MailAddress("test@gmail.com")
    let passwordValue = "VeryLongPassword1"
    let password = Password.mkPassword passwordValue |> Validation.forceSucces
    let account = {
        id = AccountId id
        credentials =
        {
            email = mailAddress
            password = password
        }
    }

    let token = Token("fake")
    let accessToken: AccessToken = {
        accountId = AccountId(Guid.NewGuid())
        value = token
        expirationUtc = DateTime.UtcNow.AddDays(1.0)
    }

    let foodstuff = {
        id = FoodstuffId(Guid.NewGuid())
        name = NonEmptyString.create "Test" |> Option.get
        baseAmount = {
            unit = MetricUnits.gram
            value = NonNegativeFloat.create 100.0 |> Option.get
        }
        amountStep =  NonNegativeFloat.create 10.0 |> Option.get
    }

    let ingredient: Ingredient = {
        FoodstuffId = foodstuff.id
        Amount = Some foodstuff.baseAmount
        Comment = None
        DisplayLine = NonEmptyString.create "test" |> Option.get
    }

    let recipe = {
        Id = RecipeId(Guid.NewGuid())
        Name = NonEmptyString.create "Test" |> Option.get
        CreatorId = Some account.id
        ImageUrl = Some <| Uri("https://google.com")
        Url = Some <| Uri("https://google.com")
        PersonCount = NaturalNumber.create 4 |> Option.get
        Description = Some (NonEmptyString.create "Test" |> Option.get)
        Ingredients = NonEmptyList.create ingredient []
        Difficulty = Some Easy
        CookingTime = None
        Tags = []
        Rating = None
        NutritionPerServing = {
            Calories = None
            Fat = None
            SaturatedFat = None
            Protein = None
            Carbs = None
            Sugars = None
            Salt = None
            Fibre = None
        }
    }

    let listItem: ListItem = {
        foodstuffId = foodstuff.id
        amount = foodstuff.baseAmount.value
    }

    let shoppingList = {
        id = ShoppingListId(Guid.NewGuid())
        accountId = account.id
        items = Map.empty
        recipes = Map.empty
    }

    // Environment

    type FakeUserDaoOptions =
        | WithUser
        | WithoutUser

    type FakeTokenDaoOptions =
        | WithToken
        | WithoutToken

    type FakeFoodstuffDaoOptions =
        | WithFoodstuff
        | WithoutFoodstuff

    type FakeEnvironment(tokenOptions, userOptions, foodstuffOptions, shoppingListItems) =

        interface IUserDao with
            member e.getById id =
                match userOptions with
                | WithUser -> Some account
                | WithoutUser -> None
            member e.getByEmail email =
                match userOptions with
                | WithUser -> Some account
                | WithoutUser -> None
            member e.add u = u

        interface ITokensDao with
            member e.get v =
                match tokenOptions with
                | WithToken -> Some accessToken
                | WithoutToken -> None
            member e.add t = t

        interface IFoodstuffDao with
            member e.getByIds ids =
                match foodstuffOptions with
                | WithFoodstuff -> seq { yield foodstuff }
                | WithoutFoodstuff -> Seq.empty
            member e.search q =
                match foodstuffOptions with
                | WithFoodstuff -> seq { yield foodstuff }
                | WithoutFoodstuff -> Seq.empty
            member e.add f = f
            member e.getVectors () = failwith "Not implemented"

        interface IRecipesDao with
            member e.getByIds ids = Seq.empty
            member e.getByAccount acc = Seq.empty
            member e.search q = Seq.empty
            member e.add r = r
            member e.getRecommendationStatistics () = failwith "Not implemented"

        interface IShoppingsListsDao with
            member e.getByAccount acc = { shoppingList with items = shoppingListItems }
            member e.update l = l
            member e.add l = l

        interface IDateTimeProvider with
            member p.nowUtc () = DateTime.UtcNow
