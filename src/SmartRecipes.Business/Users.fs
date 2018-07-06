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
        | Invalid
            
    let mkEmail s =
        try
            Success (new MailAddress(s))
        with
        | ex -> Failure [ Invalid ]
        
    (* Password *)
    
    type PasswordError =
        | MustBe10CharactersLong
        
    let private is10CharacterLong (s: string) =
        match s.Length > 10 with 
        | true -> Success <| Password s
        | false -> Failure [ MustBe10CharactersLong ]
        
    let mkPassword s =
        is10CharacterLong s
        
    (* Credentials *)
    
    type CredentialsError =
        | InvalidEmail of EmailError list
        | InvalidPassword of PasswordError list
    
    let private createCredentials email password = { email = email; password = password }
      
    let mkCredentials email pass =
        let createEmail = email |> mkEmail |> bimap (fun e -> [ InvalidEmail e ]) (fun a -> a)
        let createPassword = pass |> mkPassword  |> bimap (fun e -> [ InvalidPassword e ]) (fun a -> a)
        createCredentials 
        <!> createEmail
        <*> createPassword
        
    (* Account *)
    
    let private getId _ = AccountId <| Guid.NewGuid();
    let private createAccount credentials = { id = getId (); credentials = credentials }
    
    let mkAccount email password =
        createAccount 
        <!> mkCredentials email password