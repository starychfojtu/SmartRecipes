module Api.Users
    open FSharp.Control.Tasks
    open Giraffe
    open Microsoft.AspNetCore.Http
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
        