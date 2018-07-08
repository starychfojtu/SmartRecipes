[<RequireQualifiedAccess>]
module Business.Users
    open FSharpPlus.Data
    open System
    open System.Net.Mail
    open Models.User
    
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits of Account
        
    let signUp email password userWithGivenEmail =
        match mkAccount email password with 
        | Success a ->
            match userWithGivenEmail with
                | Some a -> AccountAlreadyExits a |> Error
                | None -> Ok a
        | Failure e -> InvalidParameters e |> Error
        