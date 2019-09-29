namespace SmartRecipes.IO

module Tokens =
    open SmartRecipes.Domain.Token
    
    type ITokensDao =
        abstract member get: string -> AccessToken option
        abstract member add: AccessToken -> AccessToken
        
    let get<'e when 'e :> ITokensDao> value = IO.operation (fun (tokens : 'e) -> tokens.get value)
    let add<'e when 'e :> ITokensDao> token = IO.operation (fun (tokens : 'e) -> tokens.add token)