namespace SmartRecipes.Api
open SmartRecipes.Domain

module Users =
    open Dto
    open SmartRecipes.UseCases
    open SmartRecipes.Domain.Credentials
    open Infrastructure
    open SmartRecipes.Domain.Token
    open Generic
    open Infrastracture
    open Errors
    open SmartRecipes.UseCases.Users
    
    module SignUp =
        type Parameters = {
            email: string
            password: string
        }
        
        type Response = {
            Account: AccountDto
        }
        
        let private serializeCredentialsError = function
            | InvalidEmail errors -> Seq.map (function Email.EmailError.Invalid -> parameterError "Email is invalid." "Email") errors
            | InvalidPassword errors -> Seq.map (function Password.PasswordError.MustBe10CharactersLong -> parameterError "Password must be at least 10 characters long." "Password") errors
        
        let private serializeError = function
            | SignUpError.AccountAlreadyExits ->  error "Account already exists."
            | SignUpError.InvalidParameters errors -> Seq.collect serializeCredentialsError errors |> invalidParameters
            
        let private serializeResponse a = {
            Account = serializeAccount a
        }

        let handler<'a> =
            postHandler (fun p -> Users.signUp p.email p.password) (Result.bimap serializeResponse serializeError)
        
    module SignIn =
        type Parameters = {
            email: string
            password: string
        }
        
        type Response = {
            AccessToken: AccessTokenDto
        }
        
        let private serializeError = function
            | InvalidCredentials -> error "Invalid credentials."
            
        let private serializeResponse token = {
            AccessToken = serializeAccessToken token
        }
             
        let handler<'a> =
            postHandler (fun p -> Users.signIn p.email p.password) (Result.bimap serializeResponse serializeError)
        