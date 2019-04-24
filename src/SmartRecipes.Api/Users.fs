module Api.Users
    open Api
    open Dto
    open DataAccess
    open Domain.Account
    open Domain.Credentials
    open Giraffe
    open Microsoft.AspNetCore.Http
    open UseCases
    open Infrastructure
    open Domain.Token
    open Generic
    open UseCases.Users
    open Api.Errors
    open Infrastracture
    
    // Sign up
    
    type SignUpParameters = {
        Email: string
        Password: string
    }
    
    type SignUpResponse = {
        Account: AccountDto
    }
    
    let getSignUpDao () = {
        users = Users.getDao ()
        shoppingLists = ShoppingLists.getDao ()
    }
    
    let private serializeCredentialsError = function
        | InvalidEmail errors -> Seq.map (function Invalid -> parameterError "Email is invalid." "Email") errors
        | InvalidPassword errors -> Seq.map (function MustBe10CharactersLong -> parameterError "Password must be at least 10 characters long." "Password") errors
    
    let private serializeSignUpError = function
        | AccountAlreadyExits ->  error "Account already exists."
        | InvalidParameters errors -> Seq.collect serializeCredentialsError errors |> invalidParameters
        
    let private serializeSignUpResponse a =
        { Account = serializeAccount a }
        
    let private serializeSignUp = 
       Result.bimap serializeSignUpResponse serializeSignUpError

    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        postHandler (getSignUpDao ()) next ctx (fun p -> Users.signUp p.Email p.Password) serializeSignUp
        
    // Sign in
        
    type SignInParameters = {
        email: string
        password: string
    }
    
    let private getSignInDao (): Users.SignInDao = {
        tokens = (Tokens.getDao ())
        users = (Users.getDao ())
    }
    
    let private serializeSignInError = function
        | InvalidCredentials -> error "Invalid credentials."
    
    let private serializeSignIn = 
        Result.map serializeAccessToken >> Result.mapError serializeSignInError
        
    let signInHandler (next : HttpFunc) (ctx : HttpContext) =
        postHandler (getSignInDao ()) next ctx (fun p -> Users.signIn p.email p.password) serializeSignIn
        