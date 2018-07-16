module Api.Users
    open System.Net.Mail
    open FSharp.Control.Tasks
    open FSharpPlus.Data
    open Giraffe
    open Microsoft.AspNetCore.Http
    open Models
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
            let result = 
                User.mkEmail parameters.email
                |> Validation.map (fun e -> Users.signIn e parameters.password)
            return! json result next ctx
        }
        