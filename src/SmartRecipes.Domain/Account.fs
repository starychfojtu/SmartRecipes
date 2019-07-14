namespace SmartRecipes.Domain

module Account =
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
    
    let private mkAccount email password =
        createAccount 
        <!> mkCredentials email password
        
    // Sign up
          
    let create email password =
        mkAccount email password
        |> Validation.toResult
