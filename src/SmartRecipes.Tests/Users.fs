module Tests.Users
    open System
    open Xunit
    open Business
    open Business
    
    [<Fact>]
    let ``Can sign up with valid parameters`` () =
        match Users.signUp "test@gmail.com" "VeryLongPassword1" (fun _ -> None) with 
        | Ok a -> ()
        | _ -> Assert.True(false)
        
        
    [<Fact>]
    let ``Cannot sign up with invalid parameters`` () =
        match Users.signUp "test" "fake" (fun _ -> None) with 
        | Error e -> 
            match e with
            | Users.SignUpError.InvalidParameters e -> Assert.Equal(e.Length, 2)
            | _ -> Assert.True(false)
        | _ -> Assert.True(false)
