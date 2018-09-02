module DataAccess.Users
    open System.Net.Mail
    open Context
    open DataAccess.Model
    open Models.Account
    open Models.Password
    open FSharpPlus.Data
    open Models.Token
    
    type UsersDao = {
        getById: AccountId -> Account option
        getByEmail: MailAddress -> Account option
        add: Account -> Account
    }
    
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
        
    let private add account =
        let ctx = createDbContext ()
        toDb account |> ctx.Add |> ignore
        ctx.SaveChanges () |> ignore
        account
        
    let private getByEmail (email: MailAddress) =
        createDbContext().Accounts 
        |> Seq.filter (fun a -> a.email = email.Address)
        |> Seq.tryHead
        |> Option.map toModel
        
    let private getById (AccountId id) =
        createDbContext().Accounts 
        |> Seq.filter (fun a -> a.id = id)
        |> Seq.tryHead
        |> Option.map toModel
            
    let getDao () = {
        getById = getById
        getByEmail = getByEmail
        add = add
    }