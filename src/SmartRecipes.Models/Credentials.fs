module Models.Credentials
    open System.Net.Mail
    open Email
    open Password
    open FSharpPlus
    open FSharpPlus.Data.Validation
    
    type Credentials = {
        email: MailAddress
        password: Password
    }

    type CredentialsError =
    | InvalidEmail of EmailError list
    | InvalidPassword of PasswordError list
        
    let private createCredentials email password = { email = email; password = password }
          
    let mkCredentials email pass =
        let createEmail = email |> mkEmail |> bimap (fun e -> [ InvalidEmail e ]) (fun a -> a)
        let createPassword = pass |> mkPassword  |> bimap (fun e -> [ InvalidPassword e ]) (fun a -> a)
        createCredentials 
        <!> createEmail
        <*> createPassword