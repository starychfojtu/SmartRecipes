module Api.Dto
    open Domain.Account
    open Domain.Token
    
    type AccountDto = {
        id: string
        email: string
    }
    
    let serializeAccount (account: Account) = {
        id = account.id.value.ToString ()
        email = account.credentials.email.Address
    }
    
    let serializeAccessToken (accessToken: AccessToken) = 
        accessToken.value