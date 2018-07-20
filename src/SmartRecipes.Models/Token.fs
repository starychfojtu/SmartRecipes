module Models.Token
    open System.Security.Cryptography
    open Account
    open System
    open System
    open System
    
    type Token = private Token of string
        
    type AccessToken = {
        accountId: AccountId
        value: Token
        expiration: DateTime
    }
    
    let private expirationTime = TimeSpan.FromDays 1.0
    
    let private generateRandomToken () =
        let bytes = Array.zeroCreate 31
        use cryptoService = new RNGCryptoServiceProvider ()
        cryptoService.GetBytes(bytes) |> ignore
        BitConverter.ToString bytes
    
    let mkAccessToken accountId = 
        let token = generateRandomToken ()
        let (AccountId s) = accountId
        let value = token + s.ToString ()
        let expiration = DateTime.UtcNow.Add expirationTime
        let accessToken = { 
            accountId = accountId
            value = Token value
            expiration = expiration
        }
        accessToken
        