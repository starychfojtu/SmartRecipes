module Models.User
    open System.Net.Mail
    open System.Text.RegularExpressions
    open System
    open FSharpPlus.Data
    open FSharpPlus
    open Infrastructure
    open Infrastructure
    
    type AccountId = AccountId of Guid
    type Password = Password of string
    
    type Credentials = {
        email: MailAddress;
        password: Password;
    }
    
    type Account = {
        id: AccountId;
        credentials: Credentials;
    }
    
    (* Email *) 
            
    type EmailError =
        | Invalid
            
    let mkEmail s =
        try
            Success (new MailAddress(s))
        with
        | ex -> Failure [ Invalid ]
        
    (* Password *)
    
    let private password s =
        Hashing.hash s |> Password
    
    type PasswordError =
            | MustBe10CharactersLong
                
    let private is10CharacterLong (s: string) =
        match s.Length > 10 with 
        | true -> Success <| s
        | false -> Failure [ MustBe10CharactersLong ]
        
    let mkPassword s =
        is10CharacterLong s
        |> Validation.map (fun s -> password s )
        
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
    
    let private createAccount credentials = { 
        id = AccountId(Guid.NewGuid()); 
        credentials = credentials 
    }
    
    let mkAccount email password =
        createAccount 
        <!> mkCredentials email password
