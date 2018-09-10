module Api.Users
    open Api
    open DataAccess
    open Giraffe
    open Microsoft.AspNetCore.Http
    open UseCases
    open Infrastructure
    open Context
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

    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        postHandler (getSignUpDao ()) next ctx (fun p -> Users.signUp p.email p.password)
        
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
        postHandler (getSignInDao ()) next ctx (fun p -> Users.signIn p.email p.password)
        