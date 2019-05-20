module Tests.Fake
    open SmartRecipes.DataAccess.Foodstuffs
    open SmartRecipes.DataAccess.Recipes
    open SmartRecipes.Domain
    open Infrastructure
    open System
    open System.Net.Mail
    open SmartRecipes.Domain.Account
    open SmartRecipes.DataAccess.Users
    open SmartRecipes.DataAccess.Tokens
    open SmartRecipes.DataAccess.ShoppingLists
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.ShoppingList
    open SmartRecipes.Domain.Token
    open FSharpPlus.Data
    open SmartRecipes.UseCases.Environment
    
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
        name = NonEmptyString.create "Test" |> Validation.forceSucces
        baseAmount = {
            unit = MetricUnits.gram
            value = NonNegativeFloat.create 100.0 |> Validation.forceSucces
        }
        amountStep =  NonNegativeFloat.create 10.0 |> Validation.forceSucces
    }
    
    let ingredient: Ingredient = {
        FoodstuffId = foodstuff.id
        Amount = Some foodstuff.baseAmount
    }
    
    let recipe = {
        Id = RecipeId(Guid.NewGuid())
        Name = NonEmptyString.create "Test" |> Validation.forceSucces
        CreatorId = account.id
        ImageUrl = Uri("https://google.com")
        PersonCount = NaturalNumber.create 4 |> Validation.forceSucces
        Description = Some (NonEmptyString.create "Test" |> Validation.forceSucces)
        Ingredients = NonEmptyList.create ingredient []
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
        
    // Dao
    
    type FakeUserDaoOptions =
        | WithUser
        | WithoutUser
    
    let usersDao options = {
        getByEmail =
            match options with
            | WithUser -> fun v -> Some account
            | WithoutUser -> fun v -> None
        getById =
            match options with
            | WithUser -> fun v -> Some account
            | WithoutUser -> fun v -> None
        add = fun a -> a
    }
    
    type FakeTokenDaoOptions =
        | WithToken
        | WithoutToken
    
    let tokensDao withToken = {
        get =
            match withToken with
            | WithToken -> fun v -> Some accessToken
            | WithoutToken -> fun v -> None
        add = fun t -> t
    }
    
    let shoppingListsDao () = {
        add = fun s -> s
        update = fun s -> s
        get = fun a -> raise (NotImplementedException())
    }
    
    type FakeFoodstuffDaoOptions =
        | WithFoodstuff
        | WithoutFoodstuff
    
    let foodstuffsDao = function
        | WithFoodstuff ->
            {
                getByIds = fun ids -> seq { yield foodstuff }
                getById = fun id -> Some foodstuff
                search =fun name -> seq { yield foodstuff }
                add = fun f -> f
            }
        | WithoutFoodstuff ->
            {
                getByIds = fun ids -> Seq.empty
                getById = fun id -> None
                search = fun name -> Seq.empty
                add = fun f -> f
            }
    
    let recipesDao: RecipesDao = {
        getByAccount = fun a -> Seq.empty
        getByIds = fun ids -> Seq.empty
        getById = fun id -> None
        add = fun r -> r
    }
    
    let shoppingListDao items: ShoppingsListsDao = {
        add = fun s -> s
        get = fun a -> { shoppingList with items = items }
        update = fun s -> s
    }

    let environment tokenOptions userOptions foodstuffOptions shoppingListItems : Environment = {
        IO = {
            Tokens = tokensDao tokenOptions
            Users = usersDao userOptions
            Recipes = recipesDao
            Foodstuffs = foodstuffsDao foodstuffOptions
            ShoppingLists = shoppingListDao shoppingListItems
        }
        NowUtc = DateTime.UtcNow
    }