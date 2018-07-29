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
    open Infrastructure.Validation
    open Infrastructure.Reader
    open Infrastructure
    
    // Sign up
    
    let private signUpAccount email password getAccontByEmail = Users.signUp email password getAccontByEmail |> Reader.id
    
    let private addAccountToDb a = Users.add a |> Reader.map Ok
    
    let signUp email password = 
        Users.getAccountByEmail
        >>= signUpAccount email password
        >>=! addAccountToDb
        |> Reader.execute (createDbContext ())
      
    // Sign in
        
    let private validateEmail email = 
        mkEmail email
        |> mapFailure (fun _ -> Users.InvalidCredentials)
        |> toResult
        |> Reader.id
        
    let private getAccount email =
        Users.getAccountByEmail
        |> Reader.map (fun getAccount -> getAccount email |> Option.toResult InvalidCredentials)
        
    let private authenticate password account =
        Users.signIn account password |> Reader.id
        
    let private addTokenToDb t = Tokens.add t |> Reader.map Ok
       
    let signIn email password =
        validateEmail email
        >>=! getAccount
        >>=! authenticate password
        >>=! addTokenToDb
        |> Reader.execute (createDbContext ())
        
    // Authorize
        
    let private verifyAccess t = Option.filter verifyAccessToken t |> Reader.id
    
    let private toError error t = (Option.toResult error) t |> Reader.id
    
    let authorize error (accessTokenValue: string) =
        Tokens.get accessTokenValue
        >>= verifyAccess
        >>= toError error
        
