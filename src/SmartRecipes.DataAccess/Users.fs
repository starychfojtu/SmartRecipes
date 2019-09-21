namespace SmartRecipes.DataAccess

module Users =
    open System.Net.Mail
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Password
    open MongoDB.Driver
    
    module Mongo =
        
        let private collection = Database.getCollection<DbAccount> ()
    
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
        
        let add account =
            collection.InsertOne (toDb account) |> ignore
            account
            
        let getByEmail (email: MailAddress) =
            collection.Find(fun a -> a.email = email.Address).ToEnumerable()
            |> Seq.tryHead
            |> Option.map toModel
            
        let getById (AccountId id) =
            collection.Find(fun a -> a.id = id).ToEnumerable()
            |> Seq.tryHead
            |> Option.map toModel