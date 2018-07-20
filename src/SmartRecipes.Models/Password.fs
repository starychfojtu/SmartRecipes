module Models.Password
    open Infrastructure
    open Hashing
    open FSharpPlus.Data

    type Password = Password of string

    let private password s =
        hash s |> Password
        
    type PasswordError =
        | MustBe10CharactersLong
                
    let private is10CharacterLong (s: string) =
        match s.Length > 10 with 
        | true -> Success <| s
        | false -> Failure [ MustBe10CharactersLong ]
        
    let mkPassword s =
        is10CharacterLong s
        |> Validation.map (fun s -> password s )