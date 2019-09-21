namespace SmartRecipes.IO

module Users =
    open System.Net.Mail
    open FSharpPlus.Data
    open SmartRecipes.Domain.Account
    
    type IUserDao = 
        abstract member getById: AccountId -> Account option
        abstract member getByEmail: MailAddress -> Account option
        abstract member add: Account -> Account
        
    let getById<'e when 'e :> IUserDao> id = Reader(fun (users : 'e) -> users.getById id)
    let getByEmail<'e when 'e :> IUserDao> email = Reader(fun (users : 'e) -> users.getByEmail email)
    let add<'e when 'e :> IUserDao> account = Reader(fun (users : 'e) -> users.add account)