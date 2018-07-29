module UseCases.Users
    open System.Net.Mail
    open Business
    open Business.Users
    open DataAccess
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Models
    open Models.Token
    open Models.Email
    open Models.Account
    open Infrastructure.Validation
    open Infrastructure.Reader
    open Infrastructure
    
    // Sign up
    
    let private verifyAccountNotExists account = 
        Users.getAccountByEmail account.credentials.email
        |> Reader.map (fun a -> match a with | Some _ -> Error AccountAlreadyExits | None -> Ok account)
            
    let private signUpAccount email password = 
        Users.signUp email password |> Reader.id
    
    let private addAccountToDb a = 
        Users.add a |> Reader.map Ok
    
    let signUp email password = 
        signUpAccount email password
        >>=! verifyAccountNotExists
        >>=! addAccountToDb
      
    // Sign in
        
    let private validateEmail email = 
        mkEmail email
        |> mapFailure (fun _ -> Users.InvalidCredentials)
        |> toResult
        |> Reader.id
        
    let private getAccount email =
        Users.getAccountByEmail email
        |> Reader.map (Option.toResult InvalidCredentials)
        
    let private authenticate password account =
        Users.signIn account password |> Reader.id
        
    let private addTokenToDb t = 
        Tokens.add t |> Reader.map Ok
       
    let signIn email password =
        validateEmail email
        >>=! getAccount
        >>=! authenticate password
        >>=! addTokenToDb
        
    // Authorize
        
    let private verifyAccess t = 
        Option.filter verifyAccessToken t |> Reader.id
    
    let private toError error t = 
        (Option.toResult error) t |> Reader.id
    
    let authorize error (accessTokenValue: string) =
        Tokens.get accessTokenValue
        >>= verifyAccess
        >>= toError error
        
