module Api.Dto
    open Domain.Account
    open Domain.Foodstuff
    open Domain.Token
    
    type AccountDto = {
        id: string
        email: string
    }
    
    let serializeAccount (account: Account) = {
        id = account.id.value.ToString ()
        email = account.credentials.email.Address
    }
    
    type AccessTokenDto = {
        value: string
        accountId: string
    }
    
    let serializeAccessToken (accessToken: AccessToken) = {
        value = accessToken.value.value
        accountId = accessToken.accountId.value.ToString ()
    }
    
    type AmountDto = {
        value: float
        unit: string
    }
    
    let serializeAmount (amount: Amount) = {
        value = amount.value.value
        unit = 
            match amount.unit with 
            | Liter -> "liter"
            | Gram -> "gram"
            | Piece -> "piece"
    }
    
    type FoodstuffDto = {
        id: string
        name: string
        amountStep: AmountDto
    }
    
    let serializeFoodstuff (foodstuff: Foodstuff) = {
        id = foodstuff.id.value.ToString ()
        name = foodstuff.name.value
        amountStep = serializeAmount foodstuff.amountStep
    }