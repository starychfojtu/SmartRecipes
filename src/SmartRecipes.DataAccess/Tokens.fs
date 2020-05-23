namespace SmartRecipes.DataAccess

open Npgsql
open Npgsql.FSharp

module Tokens =
    open System
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Token
    open MongoDB.Driver

    module Postgres =
        let readToken (read: RowReader): AccessToken = {
            accountId = AccountId <| read.uuid "accountid"
            value = Token <| read.text "value"
            expirationUtc = read.dateTime "expirationutc"
        }

        let add conn accessToken =
            conn
            |> Sql.query "INSERT INTO dbo.accesstoken VALUES (@accountid, @value, @expirationutc)"
            |> Sql.parameters [
                "accountid", Sql.uuid accessToken.accountId.value
                "value", Sql.string accessToken.value.value
                "expirationutc", Sql.timestamp accessToken.expirationUtc
            ]
            |> Sql.executeNonQuery
            |> fun _ -> accessToken

        let get conn value =
            conn
            |> Sql.query "SELECT * From dbo.accesstoken WHERE value = @value"
            |> Sql.parameters [ "value", Sql.string value ]
            |> Sql.execute readToken
            |> function | Ok l -> List.tryHead l | Error e -> failwith e.Message

    module Mongo =

        let private collection = Mongo.getCollection<DbAccessToken> ()

        let private toDb accessToken: DbAccessToken = {
            id = Guid.NewGuid()
            accountId = match accessToken.accountId with AccountId id -> id
            value = accessToken.value.value
            expiration = accessToken.expirationUtc
        }

        let private toModel (dbAccessToken: DbAccessToken): AccessToken = {
            accountId = AccountId dbAccessToken.accountId
            value = Token dbAccessToken.value
            expirationUtc = dbAccessToken.expiration
        }

        let add accessToken =
            toDb accessToken |> collection.InsertOne |> ignore
            accessToken

        let get value =
            collection.Find(fun t -> t.value = value).SortByDescending(fun t -> t.expiration :> obj).ToEnumerable()
            |> Seq.tryHead
            |> Option.map toModel