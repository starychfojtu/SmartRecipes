module DataAccess.Users
    open System.Net.Mail
    open DataAccess.Model
    open Domain.Account
    open Domain.Password
    open FSharpPlus.Data
    open Domain.Token
    open MongoRepository
    
    type UsersDao = {
        getById: AccountId -> Account option
        getByEmail: MailAddress -> Account option
        add: Account -> Account
    }
    
    let private toDb account = new DbAccount(
        match account.id with AccountId id -> id,
        account.credentials.email.Address,
        match account.credentials.password with Password p -> p
    )
    
    let private toModel (dbAccount: DbAccount): Account = {
        id = AccountId dbAccount.Id
        credentials = 
        {
            email = new MailAddress(dbAccount.Email)
            password = Password dbAccount.Password
        }
    }
    
    let private repository = new MongoRepository<DbAccount>()
        
    let private add account =
        repository.Add (toDb account) |> ignore
        account
        
    let private getByEmail (email: MailAddress) =
        repository
        |> Seq.filter (fun a -> a.Email = email.Address)
        |> Seq.tryHead
        |> Option.map toModel
        
    let private getById (AccountId id) =
        repository
        |> Seq.filter (fun a -> a.Id = id)
        |> Seq.tryHead
        |> Option.map toModel
            
    let getDao () = {
        getById = getById
        getByEmail = getByEmail
        add = add
    }