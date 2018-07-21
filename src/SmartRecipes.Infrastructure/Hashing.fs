module Infrastructure.Hashing

    let hash plain =
        BCrypt.Net.BCrypt.HashPassword(plain)
        
    let verify hashed plain = 
        BCrypt.Net.BCrypt.Verify(plain, hashed)
        
    