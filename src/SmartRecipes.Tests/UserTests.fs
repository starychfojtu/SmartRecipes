module Tests.Users
    open SmartRecipes.DataAccess
    open Xunit
    open FSharpPlus.Data
    open Infrastructure
    open SmartRecipes.UseCases
        
    let getEnvironment withToken = Fake.environment withToken false false Map.empty
        
    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getEnvironment false)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        Users.signUp "test" "fake"
        |> Reader.execute (getEnvironment false)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Cannot sign up if account already exists`` () =
        Users.signUp "test@gmail.com" "VeryLongPassword1" 
        |> Reader.execute (getEnvironment true)
        |> Tests.Assert.IsError
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        Users.signIn Fake.mailAddress.Address Fake.passwordValue
        |> Reader.execute (getEnvironment false)
        |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        Users.signIn "fake@gmail.com" "fake" 
        |> Reader.execute (getEnvironment false)
        |> Tests.Assert.IsError