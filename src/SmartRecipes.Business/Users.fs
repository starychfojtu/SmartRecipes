module Business.Users
    open FSharpPlus.Data
    open FSharpPlus.Data
    open Infrastructure
    open Models.Account
    open Models.Credentials
    open Models.Password
    open Models.Token
    open System
    open System.Net.Mail
    
    // Sign up
    
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits
          
    let signUp email password =
        mkAccount email password
        |> Validation.mapFailure InvalidParameters
        |> Validation.toResult
    
    // Sign in
    
    type SignInError = 
        | InvalidCredentials
            
    let authenticate account password =
        if Hashing.verify account.credentials.password.value password
            then mkAccessToken account.id |> Ok
            else Error InvalidCredentials
            
    // Verify access token
        
    let verifyAccessToken accessToken = 
        isFresh accessToken DateTime.UtcNow