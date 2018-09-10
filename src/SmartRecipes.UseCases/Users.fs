module UseCases.Users
    open System.Net.Mail
    open DataAccess
    open DataAccess.Tokens
    open DataAccess.Users
    open DataAccess.Context
    open DataAccess.ShoppingLists
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Domain
    open Domain.ShoppingList
    open Domain.Token
    open Domain.Email
    open Domain.Account
    open Infrastructure.Validation
    open Infrastructure.Reader
    open Infrastructure
    
    // Sign up
    
    type SignUpDao = {
        users: UsersDao
        shoppingLists: ShoppingListDao
    }
    
    let private verifyAccountNotExists account = Reader(fun (dao: SignUpDao) ->
        match dao.users.getByEmail account.credentials.email with
        | Some _ -> Error AccountAlreadyExits 
        | None -> Ok account
    )
            
    let private signUpAccount email password = 
        Account.signUp email password |> Reader.id
    
    let private addAccountToDb account = 
        Reader(fun (dao: SignUpDao) -> dao.users.add account |> Ok)
        
    let private addEmptyShoppingList account = Reader(fun (dao: SignUpDao) ->
        createShoppingList account.id |> dao.shoppingLists.add |> ignore
        Ok account
    )
    
    let signUp email password =
        signUpAccount email password
        >>=! verifyAccountNotExists
        >>=! addAccountToDb
        >>=! addEmptyShoppingList
      
    // Sign in
    
    type SignInDao = {
        tokens: TokensDao
        users: UsersDao
    }
        
    let private validateEmail email = 
        mkEmail email
        |> mapFailure (fun _ -> Token.InvalidCredentials)
        |> toResult
        |> Reader.id
        
    let private getAccount email = 
        Reader(fun (dao: SignInDao) -> dao.users.getByEmail email |> Option.toResult InvalidCredentials)
        
    let private authenticate password account =
        Token.authenticate account password |> Reader.id
        
    let private addTokenToDb token = 
        Reader(fun (dao: SignInDao) -> dao.tokens.add token |> Ok)
       
    let signIn email password =
        validateEmail email
        >>=! getAccount
        >>=! authenticate password
        >>=! addTokenToDb
        
    // Authorize

    let authorize error (accessTokenValue: string) = Reader(fun (dao: TokensDao) ->
        dao.get accessTokenValue
        |> Option.filter verifyAccessToken
        |> Option.toResult error
    )
        
