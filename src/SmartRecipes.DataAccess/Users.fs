module DataAccess.Users
    open System.Net.Mail
    open Context
    open DataAccess.Model
    open Models.Account
    open Models.Password
    open FSharpPlus.Data
    open Models.Token
    
    let private toDb account = {
        id = match account.id with AccountId id -> id
        email = account.credentials.email.Address
        password = match account.credentials.password with Password p -> p
    }
    
    let private toModel (dbAccount: DbAccount): Account = {
        id = AccountId dbAccount.id
        credentials = 
        {
            email = new MailAddress(dbAccount.email)
            password = Password dbAccount.password
        }
    }
        
    let add account =
        Reader(fun (ctx: Context) ->
            toDb account |> ctx.Add |> ignore
            ctx.SaveChanges () |> ignore
            account
        )
        
    let getAccountByEmail (email: MailAddress) = 
        Reader(fun (ctx: Context) -> 
            ctx.Accounts 
            |> Seq.filter (fun a -> a.email = email.Address)
            |> Seq.tryHead
            |> Option.map (fun a -> toModel a))
        
    let getById (AccountId id)= 
        Reader(fun (ctx: Context) ->
            ctx.Accounts 
            |> Seq.filter (fun a -> a.id = id)
            |> Seq.tryHead
            |> Option.map (fun a -> toModel a))