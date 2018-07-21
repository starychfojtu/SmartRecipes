module Business.Users
    open FSharpPlus.Data
    open Infrastructure
    open Models.Account
    open Models.Credentials
    open Models.Password
    open Models.Token
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
    
    type SignInError = 
        | InvalidCredentials
            
    let signIn account password =
        let (Password accountPassword) = account.credentials.password
        let verified = Hashing.verify accountPassword password
        match verified with
        | true -> mkAccessToken account.id |> Ok
        | false -> Error InvalidCredentials
        
    let verifyAccessToken error accessToken = 
        match Option.filter (fun t -> isFresh t DateTime.UtcNow) accessToken with
        | Some _ -> Ok ()
        | None -> Error error