module Domain.Account
    open Credentials
    open System
    open FSharpPlus
    open Infrastructure
    open FSharpPlus.Data
    
    type AccountId = AccountId of Guid 
        with member i.value = match i with AccountId v -> v
    
    type Account = {
        id: AccountId
        credentials: Credentials
    }
    
    let private createAccount credentials = { 
        id = AccountId(Guid.NewGuid()); 
        credentials = credentials 
    }
    
    let mkAccount email password =
        createAccount 
        <!> mkCredentials email password
        
    // Sign up
        
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits
          
    let signUp email password =
        mkAccount email password
        |> Validation.mapFailure InvalidParameters
        |> Validation.toResult
