module Tests.Users
    open System
    open Xunit
    open Business
    open Business
    
    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        match Users.signUp "test@gmail.com" "VeryLongPassword1" None with 
        | Ok a -> ()
        | Error e -> Assert.True(false)
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        match Users.signUp "test" "fake" None with 
        | Ok a -> Assert.True(false)
        | Error e -> 
            match e with
            | Users.SignUpError.InvalidParameters e -> Assert.Equal(e.Length, 2)
            | Users.SignUpError.AccountAlreadyExits e -> Assert.True(false)
