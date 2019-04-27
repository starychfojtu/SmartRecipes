namespace SmartRecipes.UseCases

module Users =
    open SmartRecipes.DataAccess.Tokens
    open SmartRecipes.DataAccess.Users
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure.Validation
    open Infrastructure.Reader
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
    
    let private verifyAccountDoesNotExists account = Reader(fun env ->
        match env.IO.Users.getByEmail account.credentials.email with
        | Some _ -> Error AccountAlreadyExits 
        | None -> Ok account
    )
            
    let private signUpAccount email password = 
        Account.signUp email password |> Result.mapError InvalidParameters |> Reader.id
    
    let private addAccountToDb account = 
        Reader(fun env -> env.IO.Users.add account |> Ok)
        
    let private addEmptyShoppingList account = Reader(fun env ->
        ShoppingList.create account.id |> env.IO.ShoppingLists.add |> ignore
        Ok account
    )
    
    let signUp email password =
        signUpAccount email password
        >>=! verifyAccountDoesNotExists
        >>=! addAccountToDb
        >>=! addEmptyShoppingList
      
    // Sign in
        
    let private validateEmail email = 
        Email.mkEmail email
        |> mapFailure (fun _ -> SignInError.InvalidCredentials)
        |> toResult
        |> Reader.id
        
    let private getAccount email = 
        Reader(fun env -> env.IO.Users.getByEmail email |> Option.toResult SignInError.InvalidCredentials)
        
    let private authenticate password account =
        Token.authenticate account password |> Reader.id
        
    let private addTokenToDb token = 
        Reader(fun env -> env.IO.Tokens.add token |> Ok)
       
    let signIn email password =
        validateEmail email
        >>=! getAccount
        >>=! authenticate password
        >>=! addTokenToDb
        
    // Authorize

    // TODO: Remove error parameter, do the ToResult on call site
    let authorize error (accessTokenValue: string) = Reader(fun (dao: TokensDao) ->
        dao.get accessTokenValue
        |> Option.filter Token.verify
        |> Option.map (fun t -> t.accountId)
        |> Option.toResult error
    )
    
    let authorizeWithAccount error (accessTokenValue: string) = Reader(fun (tokens: TokensDao, users: UsersDao) ->
        tokens.get accessTokenValue
        |> Option.filter Token.verify
        |> Option.bind (fun t -> users.getById t.accountId)
        |> Option.toResult error
    )
        
