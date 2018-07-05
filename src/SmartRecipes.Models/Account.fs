module Models.User
    open System.Net.Mail
    open System.Text.RegularExpressions
    open System
    open FSharpPlus.Data
    
    type AccountId = AccountId of Guid
    type Password = Password of string
    
    type Credentials = {
        email: MailAddress;
        password: Password;
    }
    
    type Account = {
        id: AccountId;
        credentials: Credentials;
    }
