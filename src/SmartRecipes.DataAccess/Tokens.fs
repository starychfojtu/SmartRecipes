module DataAccess.Tokens
    open DataAccess.Model
    open FSharpPlus.Data
    open Domain.Account
    open Domain.Token
    open System
    open MongoDB.Bson
    open MongoDB.Driver
    open MongoDB.Driver.Builders

    type TokensDao = {
        get: string -> AccessToken option
        add: AccessToken -> AccessToken
    }
    
    let private collection () = Database.getCollection<DbAccessToken> ()

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
    
    let private add accessToken =
        toDb accessToken |> collection().InsertOne |> ignore
        accessToken
        
    let private get value =
        collection().Find(fun t -> t.value = value).SortByDescending(fun t -> t.expiration :> obj).ToEnumerable()
        |> Seq.tryHead
        |> Option.map toModel
        
    let getDao () = {
        get = get
        add = add
    }