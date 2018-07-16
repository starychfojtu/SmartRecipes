module Infrastructure.Hashing
    open DevOne.Security.Cryptography.BCrypt
    
    let private salt = BCryptHelper.GenerateSalt ()

    let hash s =
        BCryptHelper.HashPassword(s, salt)
        
    let verify hashed plain = 
        BCryptHelper.CheckPassword(plain, hashed)
        
    