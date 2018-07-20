module Api.Configuration

    type SecurityConfiguration = {
        key: string
        issuer: string
    }
    
    type Configuration = {
        security: SecurityConfiguration
    }
    
    let configuration = {
        security = 
        {    
            key = "ThisIsVerySecretKey"
            issuer = "http://localhost:5000";
        }
    }