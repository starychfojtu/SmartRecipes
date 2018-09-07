module Tests.Users
    open DataAccess
    open DataAccess.Tokens
    open System.Data.SqlTypes
    open DataAccess.Users
    open System.Net.Mail
    open Domain.Account
    open Domain.Password
    open System
    open Xunit
    open FSharpPlus.Data
    open Infrastructure
    open UseCases
    open UseCases.Users
    open Utils
    
    let id = Guid.NewGuid()
    let mailAddress = new MailAddress("test@gmail.com")
    let passwordValue = "VeryLongPassword1"
    let password = mkPassword passwordValue |> forceSucces
    let account = {
        id = AccountId id
        credentials = 
        {
            email = mailAddress
            password = password
        }
    }
    
    let getFakeDao withUser = {
        getByEmail = if withUser then fun m -> Some account else fun m -> None
        getById = if withUser then fun id -> Some account else fun id -> None
        add = fun a -> a
    }
    
    let getFakeTokensDao (): TokensDao = {
        get = fun v -> None
        add = fun t -> t
    }
    
    let getFakeSignInDao (): SignInDao = {
        tokens = getFakeTokensDao ()
        users = getFakeDao true
    }

    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getFakeDao false)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        Users.signUp "test" "fake"
        |> Reader.execute (getFakeDao false)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Cannot sign up if account already exists`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getFakeDao true)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        Users.signIn mailAddress.Address passwordValue
        |> Reader.execute (getFakeSignInDao ())
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        Users.signIn "fake@gmail.com" "fake" 
        |> Reader.execute (getFakeSignInDao ())
        |> Tests.Assert.IsError