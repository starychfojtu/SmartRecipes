module Domain.Account
    open Credentials
    open System
    open FSharpPlus
    
    type AccountId = AccountId of Guid 
    
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
