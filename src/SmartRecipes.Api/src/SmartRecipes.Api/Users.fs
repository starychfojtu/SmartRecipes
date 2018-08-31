module Api.Users
    open DataAccess
    open Giraffe
    open Microsoft.AspNetCore.Http
    open UseCases
    open Infrastructure
    open Context
    open Models.Token
    
    type SignUpParameters = {
        email: string
        password: string
    }

    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<SignUpParameters>()
            let result = Users.signUp parameters.email parameters.password |> Reader.execute (createDbContext())
            return! json result next ctx
        }
        
    type SignInParameters = {
        email: string
        password: string
    }

    let signInHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<SignInParameters>()
            let result = Users.signIn parameters.email parameters.password |> Reader.execute (createDbContext())
            return!
                match result with 
                | Error e -> json e next ctx
                | Ok t -> text t.value.value next ctx
        }
        