module Api.Users

    type CredentialParameters = {
        email: string
        password: string
    }
        
    let signUpHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<CredentialParameters>()
            let result = 
                match Users.signUp parameters.email parameters.password None with
                | Ok a -> json a next ctx
                | Error e -> json e next ctx
            return! result
        }