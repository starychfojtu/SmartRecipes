module DataAccess.Users
    open System.Net.Mail
    open Database.Context
    open Models.User
    open FSharpPlus.Data
    
    let private toDbModel (account: Models.User.Account): Database.Model.Account = {
        id = match account.id with | AccountId id -> id
        email = account.credentials.email.Address
        password = match account.credentials.password with | Password p -> p
    }
    
    let private toModel (dbAccount: Database.Model.Account): Models.User.Account option =
        match mkCredentials dbAccount.email dbAccount.password with 
        | Success c -> Some { id = AccountId dbAccount.id; credentials = c }
        | Failure _ -> None
        
    let add (context: Context) account =
        toDbModel account |> context.Add |> ignore
        context.SaveChanges () |> ignore
        account
        
    let getUserByEmail (email: MailAddress) = 
        Reader(fun (ctx: Context) ->
            ctx.Accounts 
            |> Seq.filter (fun a -> a.email = email.Address)
            |> Seq.tryHead
            |> Option.bind (fun a -> toModel a)
        )