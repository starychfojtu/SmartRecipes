module Tests

open System
open Xunit
open Business

[<Fact>]
let ``My test`` () =
    Assert.True(true)
    
[<Fact>]
let ``Can sign up with valid parameters test`` () =
    match Users.signUp "test@gmail.com" "VeryLongPassword1" None with 
    | Ok a -> ()
    | Error e -> Assert.True(false)
    
    
[<Fact>]
let ``Cannot sign up with invalid parameters test`` () =
    match Users.signUp "test" "short" None with 
    | Ok a -> Assert.True(false)
    | Error e -> ()
