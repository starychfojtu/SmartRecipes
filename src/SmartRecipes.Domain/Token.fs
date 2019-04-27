namespace SmartRecipes.Domain

module Token =
    open System.Security.Cryptography
    open Account
    open FSharpPlus
    open Infrastructure
    open System
    
    type Token = Token of string
        with member t.value = match t with (Token v) -> v
        
    type AccessToken = {
        accountId: AccountId
        value: Token
        expirationUtc: DateTime
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
        let expirationUtc = DateTime.UtcNow.Add expirationTime
        let accessToken = { 
            accountId = accountId
            value = Token value
            expirationUtc = expirationUtc
        }
        accessToken
        
    let isFresh accessToken nowUtc = 
        nowUtc < accessToken.expirationUtc
        
    // Sign in
        
    type SignInError = 
        | InvalidCredentials
            
    let authenticate account password =
        if Hashing.verify account.credentials.password.value password
            then mkAccessToken account.id |> Ok
            else Error InvalidCredentials
            
    // Verify access token
        
    let verify accessToken = 
        isFresh accessToken DateTime.UtcNow
        