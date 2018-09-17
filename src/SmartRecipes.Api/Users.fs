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
    
    // Sign up
    
    type SignUpParameters = {
        email: string
        password: string
    }
    
    let getSignUpDao () = {
        users = Users.getDao ()
        shoppingLists = ShoppingLists.getDao ()
    }
    
    let private serializeCredentialsError = function
        | InvalidEmail errors -> Seq.map (fun e -> match e with Invalid -> "Email is invalid.") errors
        | InvalidPassword errors -> Seq.map (fun e -> match e with MustBe10CharactersLong -> "Password mus be at least 10 characters long.") errors
    
    let private serializeSignUpError = function
        | AccountAlreadyExits -> ["Account already exists."]
        | InvalidParameters errors -> Seq.collect serializeCredentialsError errors |> Seq.toList
        
    let private serialize = 
       Result.map serializeAccount >> Result.mapError serializeSignUpError

    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        postHandler (getSignUpDao ()) next ctx (fun p -> Users.signUp p.email p.password) serialize
        
    // Sign in
        
    type SignInParameters = {
        email: string
        password: string
    }
    
    let private getSignInDao (): Users.SignInDao = {
        tokens = (Tokens.getDao ())
        users = (Users.getDao ())
    }
        
    let signInHandler (next : HttpFunc) (ctx : HttpContext) =
        postHandler (getSignInDao ()) next ctx (fun p -> Users.signIn p.email p.password) (fun a -> a)
        