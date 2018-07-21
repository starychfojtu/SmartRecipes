module UseCases.Users
    open System.Net.Mail
    open Business
    open Business.Users
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Models
    open Models.Token
    open Models.Email
    open Infrastructure.Validation
    open Infrastructure
    
    let signUp email password = 
        Users.getAccountByEmail
        |> Reader.map (Users.signUp email password)
        |> Reader.bindResult (fun a -> Users.add a |> Reader.map (fun a -> Ok a))
        |> Reader.execute (createDbContext ())
        
    let private validateEmail email = 
        mkEmail email
        |> mapFailure (fun e -> Users.InvalidCredentials)
        |> toResult
        
    let private getAccount email =
        Users.getAccountByEmail
        |> Reader.map
            (fun getAccount ->
                match getAccount email with
                | Some a -> Ok a
                | None -> Error InvalidCredentials)
        
    let private authenticate password account =
        Users.signIn account password
       
    let signIn email password =
        validateEmail email
        |> Reader.id
        |> Reader.bindResult getAccount
        |> Reader.map (fun r -> Result.bind (authenticate password) r)
        // TODO: Add token to db
        |> Reader.execute (createDbContext ())
        
