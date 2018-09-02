module DataAccess.Tokens
    open DataAccess.Context
    open DataAccess.Model
    open FSharpPlus.Data
    open Models.Account
    open Models.Token

    type TokensDao = {
        get: string -> Reader<Context, AccessToken option>
        add: AccessToken -> Reader<Context, AccessToken>
    }

    let private toDb accessToken: DbAccessToken = {
        accountId = match accessToken.accountId with AccountId id -> id
        value = accessToken.value.value
        expiration = accessToken.expiration
    }
        
    let private toModel (dbAccessToken: DbAccessToken): AccessToken = { 
        accountId = AccountId dbAccessToken.accountId
        value = Token dbAccessToken.value
        expiration = dbAccessToken.expiration
    }
    
    let private add accessToken =
        Reader(fun (ctx: Context) ->
            toDb accessToken |> ctx.Add |> ignore
            ctx.SaveChanges () |> ignore
            accessToken
        )
        
    let private get value =
        Reader(fun (ctx: Context) -> 
            ctx.AccessTokens 
            |> Seq.filter (fun t -> t.value = value)
            |> Seq.sortByDescending (fun t -> t.expiration)
            |> Seq.tryHead
            |> Option.map (fun t -> toModel t)
        )
        
    let getDao () = {
        get = get
        add = add
    }