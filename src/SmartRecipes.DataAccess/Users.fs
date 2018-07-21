module DataAccess.Users
    open System.Net.Mail
    open Database.Context
    open Models.Account
    open Models.Password
    open FSharpPlus.Data
    
    let private toDbModel (account: Models.Account.Account): Database.Model.Account = {
        id = match account.id with | AccountId id -> id
        email = account.credentials.email.Address
        password = match account.credentials.password with | Password p -> p
    }
    
    let private toModel (dbAccount: Database.Model.Account): Models.Account.Account = { 
        id = AccountId dbAccount.id
        credentials = 
        {
            email = new MailAddress(dbAccount.email)
            password = Password dbAccount.password
        }
    }
        
    let add account =
        Reader(fun (ctx: Context) ->
            toDbModel account |> ctx.Add |> ignore
            ctx.SaveChanges () |> ignore
            account
        )
        
    let getAccountByEmail = 
        Reader(fun (ctx: Context) -> (fun (email: MailAddress) ->
            ctx.Accounts 
            |> Seq.filter (fun a -> a.email = email.Address)
            |> Seq.tryHead
            |> Option.map (fun a -> toModel a)
        ))