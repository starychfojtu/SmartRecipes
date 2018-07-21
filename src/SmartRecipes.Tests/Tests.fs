module Tests
    open System.Net.Mail
    open Business.Users
    open Models.Account
    open Models.Password
    open System
    open Xunit
    
    let AssertFail () = Assert.True(false)
    
    let id = Guid.NewGuid()
    let mailAddress = new MailAddress("test@gmail.com")
    let passwordValue = "VeryLongPassword1"
    let password = Password passwordValue
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
        match signUp "test@gmail.com" "VeryLongPassword1" (fun _ -> None) with 
        | Ok a -> ()
        | _ -> AssertFail ()
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        match signUp "test" "fake" (fun _ -> None) with 
        | Error e -> 
            match e with
            | SignUpError.InvalidParameters e -> Assert.Equal(e.Length, 2)
            | _ -> AssertFail ()
        | _ -> AssertFail ()
        
        
    [<Fact>]
    let ``Can sign in with valid password`` () =
        match signIn account passwordValue with 
        | Ok a -> ()
        | _ -> AssertFail ()
        
        
    [<Fact>]
    let ``Cannot sign in with invalid password`` () =
        match signIn account "fake" with 
        | Error e -> ()
        | _ -> AssertFail ()