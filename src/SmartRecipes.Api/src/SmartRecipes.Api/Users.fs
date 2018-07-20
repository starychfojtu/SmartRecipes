module Api.Users
    open System.Security.Claims
    open System.Text
    open System.IdentityModel.Tokens.Jwt
    open System.Net.Mail
    open FSharp.Control.Tasks
    open FSharpPlus.Data
    open Giraffe
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Configuration
    open Microsoft.IdentityModel.Tokens
    open Models
    open System
    open UseCases
    
    type SignUpParameters = {
        email: string
        password: string
    }

    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<SignUpParameters>()
            let result = Users.signUp parameters.email parameters.password
            return! json result next ctx
        }
        
    type SignInParameters = {
        email: string
        password: string
    }
        
    let signInHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<SignInParameters>()
            let isAuthenticated = 
                User.mkEmail parameters.email
                |> Validation.map (fun e -> Users.signIn e parameters.password)
            
            return! text encodedToken next ctx
        }
        