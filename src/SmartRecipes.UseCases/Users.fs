namespace SmartRecipes.UseCases

module Users =
    open SmartRecipes.DataAccess.Tokens
    open SmartRecipes.DataAccess.Users
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure.Validation
    open Infrastructure
    open Environment
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Credentials
    open SmartRecipes.Domain.Token
    
    // Sign up
    
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits

    let private verifyAccountNotExists account =
        Users.getByEmail account.credentials.email
        |> ReaderT.mapDirect (function | Some _ -> Error AccountAlreadyExits | None -> Ok account)
            
    let private signUpAccount email password =
        Account.create email password |> Result.mapError InvalidParameters |> ReaderT.id
    
    let private addAccountToDb account = 
        Users.add account |> ReaderT.mapDirect Ok
        
    let private addEmptyShoppingList account =
        ShoppingList.create account.id
        |> ShoppingList.add
        |> ReaderT.mapDirect (fun _ -> Ok account)
    
    let signUp email password =
        signUpAccount email password
        >>= verifyAccountNotExists
        >>= addAccountToDb
        >>= addEmptyShoppingList
      
    // Sign in
        
    let private validateEmail email = 
        Email.mkEmail email
        |> mapFailure (fun _ -> SignInError.InvalidCredentials)
        |> toResult
        |> ReaderT.id
        
    let private getAccount email = 
        ReaderT(fun env -> env.IO.Users.getByEmail email |> Option.toResult SignInError.InvalidCredentials)
        
    let private authenticate password account = ReaderT(fun env ->
        Token.authenticate env.NowUtc account password)
        
    let private addTokenToDb token = 
        ReaderT(fun env -> env.IO.Tokens.add token |> Ok)
       
    let signIn email password =
        validateEmail email
        >>= getAccount
        >>= authenticate password
        >>= addTokenToDb
        
    // Authorize

    let authorize error (accessTokenValue: string) = ReaderT(fun env ->
        env.IO.Tokens.get accessTokenValue
        |> Option.filter (Token.isFresh env.NowUtc)
        |> Option.map (fun t -> t.accountId)
        |> Option.toResult error
    )
    
    let authorizeWithAccount error (accessTokenValue: string) =
        authorize error accessTokenValue
        >>= fun id -> ReaderT(fun env -> env.IO.Users.getById id |> Ok)