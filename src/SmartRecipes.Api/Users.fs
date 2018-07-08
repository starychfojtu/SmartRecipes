module Api.Users
    open Business
    open Database
    open FSharp.Control.Tasks
    open Giraffe
    open Microsoft.AspNetCore.Http
       
    type SignUpParameters = {
        email: string
        password: string
    }
        
    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<SignUpParameters>()
            let result = 
                match Users.signUp parameters.email parameters.password None with
                | Ok a -> json a next ctx
                | Error e -> json e next ctx
            return! result
        }