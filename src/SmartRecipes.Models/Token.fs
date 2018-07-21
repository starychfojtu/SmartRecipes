module Models.Token
    open System.Globalization
    open System.Security.Cryptography
    open Account
    open FSharpPlus
    open System
    open System
    open System
    
    type Token = Token of string
            
    let value (Token v) = v
        
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
        |> String.replace "-" ""
        |> String.toLower
    
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
        
    let isFresh accessToken nowUtc = 
        nowUtc < accessToken.expiration
        