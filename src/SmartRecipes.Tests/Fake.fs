module Tests.Fake
    open DataAccess.Foodstuffs
    open Domain
    open Infrastructure
    open System
    open System.Net.Mail
    open Domain.Account
    open DataAccess.Users
    open DataAccess.Tokens
    open DataAccess.ShoppingLists
    open Domain
    open Domain.NonEmptyString
    open Domain.NonNegativeFloat
    open Domain.Foodstuff
    open Domain.Token
    open Infrastructure
    open UseCases.Users
    
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
        accountId = AccountId(Guid.Empty)
        value = token
        expiration = DateTime.UtcNow.AddDays(1.0)
    }
    let foodstuff: Foodstuff = {
        id = FoodstuffId(Guid.Empty)
        name = mkNonEmptyString "Test" |> Validation.forceSucces
        baseAmount = {
            unit = Gram
            value = mkNonNegativeFloat 100.0 |> Validation.forceSucces
        }
        amountStep = {
            unit = Gram
            value = mkNonNegativeFloat 10.0 |> Validation.forceSucces
        }
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
        search = if withFoodstuff then fun name -> seq { yield foodstuff } else fun name -> Seq.empty
        add = fun f -> f
    }