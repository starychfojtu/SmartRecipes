module Tests.Users
    open SmartRecipes.DataAccess
    open Xunit
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes.UseCases
    open Fake
        
    let getEnvironment withUser =
        Fake.environment WithoutToken withUser WithoutFoodstuff Map.empty
        
    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> ReaderT.execute (getEnvironment WithoutUser)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        Users.signUp "test" "fake"
        |> ReaderT.execute (getEnvironment WithoutUser)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Cannot sign up if account already exists`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> ReaderT.execute (getEnvironment WithUser)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        Users.signIn Fake.mailAddress.Address Fake.passwordValue
        |> ReaderT.execute (getEnvironment WithUser)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        Users.signIn "fake@gmail.com" "fake" 
        |> ReaderT.execute (getEnvironment WithoutUser)
        |> Tests.Assert.IsError