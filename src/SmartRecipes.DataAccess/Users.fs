module DataAccess.Users
    open DataAccess
    open System.Net.Mail
    open DataAccess.Model
    open Domain.Account
    open Domain.Password
    open FSharpPlus.Data
    open Domain.Token
    open MongoDB.Bson
    open MongoDB.Driver
    
    type UsersDao = {
        getById: AccountId -> Account option
        getByEmail: MailAddress -> Account option
        add: Account -> Account
    }
    
    let private collection () = Database.getCollection<DbAccount> ()
    
    let private toDb account: DbAccount = {
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

    let private add account =
        collection().InsertOne (toDb account) |> ignore
        account
        
    let private getByEmail (email: MailAddress) =
        collection().Find(fun a -> a.email = email.Address).ToEnumerable()
        |> Seq.tryHead
        |> Option.map toModel
        
    let private getById (AccountId id) =
        collection().Find(fun a -> a.id = id).ToEnumerable()
        |> Seq.tryHead
        |> Option.map toModel
            
    let getDao () = {
        getById = getById
        getByEmail = getByEmail
        add = add
    }