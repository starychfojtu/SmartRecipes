namespace SmartRecipes.UseCases

module Users =
    open SmartRecipes.IO
    open FSharpPlus
    open FSharpPlus.Data
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
        |> IO.fromResult
            
    let private verifyAccountNotExists account =
        Users.getByEmail account.credentials.email
        |> IO.toEIO (fun existingAccount ->
            match existingAccount with
            | Some _ -> Error AccountAlreadyExits
            | None -> Ok account)
            
    let private addAccountToDb account = 
        Users.add account
        |> IO.toEIO (fun _ -> Ok account)
        
    let private addEmptyShoppingList account =
        ShoppingList.create account.id
        |> ShoppingLists.add
        |> IO.toEIO (fun _ -> Ok account)
    
    let signUp email password =
        signUpAccount email password
        >>= verifyAccountNotExists
        >>= addAccountToDb
        >>= addEmptyShoppingList
      
    // Sign in
        
    let private validateEmail email = 
        Email.mkEmail email
        |> Validation.mapFailure (fun _ -> SignInError.InvalidCredentials)
        |> Validation.toResult
        |> IO.fromResult
        
    let private getAccount email = 
        Users.getByEmail email 
        |> IO.toEIO (Option.toResult SignInError.InvalidCredentials)
        
    let private authenticate password account =
        DateTimeProvider.nowUtc
        |> IO.toEIO (fun nowUtc -> Token.authenticate nowUtc account password)
       
    let signIn email password =
        validateEmail email
        >>= getAccount
        >>= authenticate password
        >>= (Tokens.add >> IO.toSuccessEIO)
        
    // Authorize

    let authorize error (accessTokenValue: string) =
        let result = monad {
            let! token = Tokens.get accessTokenValue
            let! nowUtc = DateTimeProvider.nowUtc
            return token
                |> Option.filter (Token.isFresh nowUtc)
                |> Option.map (fun t -> t.accountId)
        }
        IO.toEIO (Option.toResult error) result
        
    let authorizeWithAccount error (accessTokenValue: string) =
        authorize error accessTokenValue
        >>= (Users.getById >> IO.toSuccessEIO)