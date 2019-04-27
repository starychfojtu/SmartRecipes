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
        name = mkNonEmptyString "Test" |> Validation.forceSucces
        baseAmount = {
            unit = Gram
            value = NonNegativeFloat.create 100.0 |> Validation.forceSucces
        }
        amountStep = {
            unit = Gram
            value = NonNegativeFloat.create 10.0 |> Validation.forceSucces
        }
    }
    
    let ingredient: Ingredient = {
        foodstuffId = foodstuff.id
        amount = foodstuff.baseAmount.value
    }
    
    let recipe = {
        id = RecipeId(Guid.NewGuid())
        name = mkNonEmptyString "Test" |> Validation.forceSucces
        creatorId = account.id
        imageUrl = Uri("https://google.com")
        personCount = NaturalNumber.create 4 |> Validation.forceSucces
        description = Some (mkNonEmptyString "Test" |> Validation.forceSucces)
        ingredients = NonEmptyList.create ingredient []
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
    
    let usersDao withUser = {
        getByEmail = if withUser then fun m -> Some account else fun m -> None
        getById = if withUser then fun id -> Some account else fun id -> None
        add = fun a -> a
    }
    
    let tokensDao withToken = {
        get = if withToken then fun v -> Some accessToken else fun v -> None
        add = fun t -> t
    }
    
    let shoppingListsDao () = {
        add = fun s -> s
        update = fun s -> s
        get = fun a -> raise (NotImplementedException())
    }
    
    let foodstuffsDao withFoodstuff = {
        getByIds = if withFoodstuff then fun ids -> seq { yield foodstuff } else fun ids -> Seq.empty
        getById = if withFoodstuff then fun id -> Some foodstuff else fun id -> None
        search = if withFoodstuff then fun name -> seq { yield foodstuff } else fun name -> Seq.empty
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
    
    let environment withToken withUser withFoodstuff items : Environment = {
        IO = {
            Tokens = tokensDao withToken
            Users = usersDao withUser
            Recipes = recipesDao
            Foodstuffs = foodstuffsDao withFoodstuff
            ShoppingLists = shoppingListDao items
        }
        NowUtc = DateTime(2019, 5, 5)
    }