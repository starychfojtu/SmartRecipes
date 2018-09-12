module Tests.Users
    open DataAccess
    open DataAccess.ShoppingLists
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
    
    let getFakeSignUpDao withUser: SignUpDao = {
        users = Fake.usersDao withUser
        shoppingLists = Fake.shoppingListsDao ()
    }
    
    let getFakeSignInDao (): SignInDao = {
        tokens = Fake.tokensDao false
        users = Fake.usersDao true
    }
        
    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getFakeSignUpDao false)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        Users.signUp "test" "fake"
        |> Reader.execute (getFakeSignUpDao false)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Cannot sign up if account already exists`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getFakeSignUpDao true)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        Users.signIn Fake.mailAddress.Address Fake.passwordValue
        |> Reader.execute (getFakeSignInDao ())
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        Users.signIn "fake@gmail.com" "fake" 
        |> Reader.execute (getFakeSignInDao ())
        |> Tests.Assert.IsError