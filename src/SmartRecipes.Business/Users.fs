[<RequireQualifiedAccess>]
module Business.Users
    open FSharpPlus.Data
    open System
    open System.Net.Mail
    open Models.User
    
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
        