[<RequireQualifiedAccess>]
module Business.Users
open FSharpPlus.Data
open FSharpPlus.Data
open System
open System.Net.Mail
    
    type EmailError =
        | MustContaintAt
        | NonEmpty
        
    type PasswordError =
        | NonEmpty
        | MustCotainNumber
    
    type CredentialsError =
        | EmailError of EmailError
        | PasswordError of PasswordError
    
    type ValidationMessage = ValidationMessage of string
        
    let (|NullOrEmpty|NotNullNorEmpty|) s = 
        match String.IsNullOrEmpty s with 
        | true -> NullOrEmpty
        | false -> NotNullNorEmpty
        
    let validatePassword password =
        match password with 
        | NullOrEmpty -> Failure(PasswordError NonEmpty)
        | NotNullNorEmpty -> Success password
        
    let tryCreateEmail s = 
        try
            let mail = new MailAddress(s)
            Some(mail)
        with
        | ex -> None
        
    let validateEmail email =
        match tryCreateEmail email with 
        | Some mail -> Success mail
        | None -> Failure(ValidationMessage "Invalid email address.")
        
    let validateCredentials credentials =
        validateEmail credentials.email
        >=> validatePassword 
            ()