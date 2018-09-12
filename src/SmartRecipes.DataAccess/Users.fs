module DataAccess.Users
    open System.Net.Mail
    open DataAccess.Model
    open Domain.Account
    open Domain.Password
    open FSharpPlus.Data
    open Domain.Token
    
    type UsersDao = {
        getById: AccountId -> Account option
        getByEmail: MailAddress -> Account option
        add: Account -> Account
    }
    
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
        account
        
//    let private getByEmail (email: MailAddress) =
//        repository
//        |> Seq.filter (fun a -> a.Email = email.Address)
//        |> Seq.tryHead
//        |> Option.map toModel
//        
//    let private getById (AccountId id) =
//        repository
//        |> Seq.filter (fun a -> a.Id = id)
//        |> Seq.tryHead
//        |> Option.map toModel
            
    let getDao () = {
        getById = fun id -> None
        getByEmail = fun email -> None
        add = add
    }