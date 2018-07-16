[<RequireQualifiedAccess>]
module Business.Users
    open DevOne.Security.Cryptography.BCrypt
    open FSharpPlus.Data
    open Infrastructure
    open Models.User
    open System
    open System.Net.Mail
    
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits of Account
            
    let signUp email password getUserByEmail =
        match mkAccount email password with 
        | Success a ->
            match getUserByEmail a.credentials.email with
                | Some a -> AccountAlreadyExits a |> Error
                | None -> Ok a
        | Failure e -> InvalidParameters e |> Error 
    
    let signIn (account : Account) password =
        match account.credentials.password with
        | Password p -> Hashing.verify p password
        
        