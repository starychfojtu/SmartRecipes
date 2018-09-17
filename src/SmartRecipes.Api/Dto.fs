module Api.Dto
    open Domain.Account
    open Domain.Token
    
    let serializeAccount (account: Account) =
        account
    
    let serializeAccessToken (accessToken: AccessToken) = 
        accessToken.value