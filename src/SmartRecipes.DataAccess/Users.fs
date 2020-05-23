namespace SmartRecipes.DataAccess

open Npgsql.FSharp

module Users =
    open System.Net.Mail
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Password
    open MongoDB.Driver

    module Postgres =
        let readAccount (read: RowReader): Account = {
            id = AccountId <| read.uuid "id"
            credentials =
            {
                email = MailAddress <| read.string "email"
                password = Password <| read.string "password"
            }
        }

        let add conn (account: Account) =
            conn
            |> Sql.query "INSERT INTO dbo.account VALUES (@id, @email, @password)"
            |> Sql.parameters [
                "id", Sql.uuid account.id.value
                "email", Sql.string account.credentials.email.Address
                "password", Sql.string account.credentials.password.value
            ]
            |> Sql.executeNonQuery
            |> fun _ -> account

        let getByEmail conn (email: MailAddress) =
            conn
            |> Sql.query "SELECT * From dbo.account WHERE email = @email"
            |> Sql.parameters [ "email", Sql.string email.Address ]
            |> Sql.execute readAccount
            |> function | Ok l -> List.tryHead l | Error e -> failwith e.Message

        let getById conn (AccountId id) =
            conn
            |> Sql.query "SELECT * From dbo.account WHERE id = @id"
            |> Sql.parameters [ "email", Sql.uuid id ]
            |> Sql.execute readAccount
            |> function | Ok l -> List.tryHead l | Error e -> failwith e.Message

    module Mongo =

        let private collection = Mongo.getCollection<DbAccount> ()

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