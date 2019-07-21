namespace SmartRecipes.DataAccess

module Tokens =
    open System
    open FSharpPlus.Data
    open Model
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Token
    open MongoDB.Driver
    
    type ITokensDao =
        abstract member get: string -> AccessToken option
        abstract member add: AccessToken -> AccessToken
        
    let get<'e when 'e :> ITokensDao> value = Reader(fun (tokens : 'e) -> tokens.get value)
    let add<'e when 'e :> ITokensDao> token = Reader(fun (tokens : 'e) -> tokens.add token)
    
    module Mongo = 
   
        let private collection = Database.getCollection<DbAccessToken> ()

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