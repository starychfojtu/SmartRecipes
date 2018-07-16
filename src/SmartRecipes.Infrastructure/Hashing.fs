module Infrastructure.Hashing
    open DevOne.Security.Cryptography.BCrypt
    open DevOne.Security.Cryptography.BCrypt

    let hash s =
        BCryptHelper.HashPassword(s, "")

    let verify hashed plain = 
        BCryptHelper.CheckPassword(plain, hashed)