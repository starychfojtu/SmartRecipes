module Tests.Users
    open System.Net.Mail
    open Business.Users
    open Models.Account
    open Models.Password
    open System
    open Xunit
    open FSharpPlus.Data
    
    let id = Guid.NewGuid()
    let mailAddress = new MailAddress("test@gmail.com")
    let passwordValue = "VeryLongPassword1"
    let password = 
        match mkPassword passwordValue with 
        | Success s -> s 
        | Failure _ -> raise (InvalidOperationException ("Test is invalidly configured.")) 
    let account = {
        id = AccountId id
        credentials = 
        {
            email = mailAddress
            password = password
        }
    }

    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        signUp "test@gmail.com" "VeryLongPassword1" |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        signUp "test" "fake" |> Tests.Assert.IsError
        
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        signIn account passwordValue |> Tests.Assert.IsOk
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        signIn account "fake" |> Tests.Assert.IsError