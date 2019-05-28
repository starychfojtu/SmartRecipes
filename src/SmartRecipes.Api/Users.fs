namespace SmartRecipes.Api
open SmartRecipes.Domain
open SmartRecipes.Domain
open SmartRecipes.Domain.Email

module Users =
    open Dto
    open SmartRecipes.UseCases
    open SmartRecipes.Domain.Credentials
    open Giraffe
    open Microsoft.AspNetCore.Http
    open Infrastructure
    open SmartRecipes.Domain.Token
    open Generic
    open Infrastracture
    open Errors
    open SmartRecipes.UseCases.Users
    
    // Sign up
    
    type SignUpParameters = {
        Email: string
        Password: string
    }
    
    type SignUpResponse = {
        Account: AccountDto
    }
    
    let private serializeCredentialsError = function
        | InvalidEmail errors -> Seq.map (function Email.EmailError.Invalid -> parameterError "Email is invalid." "Email") errors
        | InvalidPassword errors -> Seq.map (function Password.PasswordError.MustBe10CharactersLong -> parameterError "Password must be at least 10 characters long." "Password") errors
    
    let private serializeSignUpError = function
        | SignUpError.AccountAlreadyExits ->  error "Account already exists."
        | SignUpError.InvalidParameters errors -> Seq.collect serializeCredentialsError errors |> invalidParameters
        
    let private serializeSignUpResponse a =
        { Account = serializeAccount a }
        
    let private serializeSignUp = 
       Result.bimap serializeSignUpResponse serializeSignUpError

    let signUpHandler<'a> =
        postHandler (fun p -> Users.signUp p.Email p.Password) serializeSignUp
        
    // Sign in
        
    type SignInParameters = {
        Email: string
        Password: string
    }
    
    type SignInResponse = {
        AccessToken: AccessTokenDto
    }
    
    let private serializeSignInError = function
        | InvalidCredentials -> error "Invalid credentials."
        
    let private serializeSignInResponse token =
         { AccessToken = serializeAccessToken token }
    
    let private serializeSignIn = 
        Result.bimap serializeSignInResponse serializeSignInError
        
    let signInHandler<'a> =
        postHandler (fun p -> Users.signIn p.Email p.Password) serializeSignIn
        