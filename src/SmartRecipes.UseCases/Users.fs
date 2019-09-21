namespace SmartRecipes.UseCases

module Users =
    open SmartRecipes.IO
    open FSharpPlus
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure.Validation
    open Infrastructure
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Credentials
    open SmartRecipes.Domain.Token

    // Sign up
    
    type SignUpError = 
        | InvalidParameters of CredentialsError list
        | AccountAlreadyExits
    
    let private signUpAccount email password =
        Account.create email password 
        |> Result.mapError InvalidParameters 
        |> SmartRecipes.IO.IO.fromResult
            
    let private verifyAccountNotExists account =
        Users.getByEmail account.credentials.email
        |> Reader.map (function | Some _ -> Error AccountAlreadyExits | None -> Ok account)
        |> ReaderT.fromReader
            
    let private addAccountToDb account = 
        Users.add account |> IO.success
        
    let private addEmptyShoppingList account =
        let shoppingList = ShoppingList.create account.id
        ShoppingLists.add shoppingList
        |> Reader.map (fun _ -> Ok account)
        |> ReaderT.fromReader
    
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
        Users.getByEmail email 
        |> Reader.map (Option.toResult SignInError.InvalidCredentials) 
        |> ReaderT.fromReader
        
    let private authenticate password account =
        DateTimeProvider.nowUtc
        |> Reader.map (fun nowUtc -> Token.authenticate nowUtc account password)
        |> ReaderT.fromReader
        
    let private addTokenToDb token = 
        Tokens.add token |> ReaderT.hoistOk
       
    let signIn email password =
        validateEmail email
        >>= getAccount
        >>= authenticate password
        >>= addTokenToDb
        
    // Authorize

    let authorize error (accessTokenValue: string) = 
        let result = Builders.monad {
            let! token = Tokens.get accessTokenValue
            let! nowUtc = DateTimeProvider.nowUtc
            return token
                |> Option.filter (Token.isFresh nowUtc)
                |> Option.map (fun t -> t.accountId)
                |> Option.toResult error
        }
        ReaderT.fromReader result 
   
    let authorizeWithAccount error (accessTokenValue: string) =
        authorize error accessTokenValue
        >>= fun id -> Users.getById id |> ReaderT.hoistOk