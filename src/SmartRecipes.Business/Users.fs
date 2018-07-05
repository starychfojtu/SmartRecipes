[<RequireQualifiedAccess>]
module Business.Users
    open FSharpPlus
    open FSharpPlus.Data
    open System
    open System.Net.Mail
    open Models.User
    open Validation
    
    
    (* Email *) 
        
    type EmailError =
        | InvalidMailAddress
            
    let mkEmail s =
        try
            Success (new MailAddress(s))
        with
        | ex -> Failure [ InvalidMailAddress ]
        
    (* Password *)
    
    type PasswordError =
        | MustBe10CharactersLong
        
    let private is10CharacterLong (s: string) =
        match s.Length > 10 with 
        | true -> Success <| Password s
        | false -> Failure [  MustBe10CharactersLong ]
        
    let mkPassword s =
        is10CharacterLong s
        
    (* Credentials *)
    
    type CredentialsError =
        | EmailError of EmailError
        | PasswordError of PasswordError
        
    let mkCredentials email pass =
        let mailAddress = mkEmail email
        let b = map mailAddress (fun ma -> ma)
        let password = mkEmail pass
        let a = apply mailAddress password
        ()