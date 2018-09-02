module UseCases.Users
    open System.Net.Mail
    open Business
    open Business.Users
    open DataAccess
    open DataAccess.Tokens
    open DataAccess.Users
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
    
    let private verifyAccountNotExists usersDao account = 
        usersDao.getByEmail account.credentials.email
        |> Reader.map (fun a -> match a with | Some _ -> Error AccountAlreadyExits | None -> Ok account)
            
    let private signUpAccount email password = 
        Users.signUp email password |> Reader.id
    
    let private addAccountToDb usersDao = 
        usersDao.add >> Reader.map Ok
    
    let signUp usersDao email password = 
        signUpAccount email password
        >>=! verifyAccountNotExists usersDao
        >>=! addAccountToDb usersDao
      
    // Sign in
        
    let private validateEmail email = 
        mkEmail email
        |> mapFailure (fun _ -> Users.InvalidCredentials)
        |> toResult
        |> Reader.id
        
    let private getAccount usersDao email =
        usersDao.getByEmail email
        |> Reader.map (Option.toResult InvalidCredentials)
        
    let private authenticate password account =
        Users.authenticate account password |> Reader.id
        
    let private addTokenToDb (tokensDao: TokensDao) = 
        tokensDao.add >> Reader.map Ok
       
    let signIn usersDao tokensDao email password =
        validateEmail email
        >>=! getAccount usersDao
        >>=! authenticate password
        >>=! addTokenToDb tokensDao
        
    // Authorize
        
    let private verifyAccess t = 
        Option.filter verifyAccessToken t |> Reader.id
    
    let private toError error t = 
        (Option.toResult error) t |> Reader.id
    
    let authorize tokensDao error (accessTokenValue: string) =
        tokensDao.get accessTokenValue
        >>= verifyAccess
        >>= toError error
        
