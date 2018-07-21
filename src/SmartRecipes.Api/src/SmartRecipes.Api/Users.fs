module Api.Users
    open System
    open System.Text
    open System.Net.Mail
    open FSharp.Control.Tasks
    open FSharpPlus.Data
    open Giraffe
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Configuration
    open Models
    open Models.Email
    open Models.Token
    open UseCases
    open UseCases.Users
    open Business.Users
    
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
            let result = Users.signIn parameters.email parameters.password
            return!
                match result with 
                | Error e -> json InvalidCredentials next ctx
                | Ok t -> text (value t.value) next ctx
        }
        