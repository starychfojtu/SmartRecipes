namespace SmartRecipes.Models

open System

type AccountId = Guid
type Email = string
type Password = string

[<CLIMutable>]
type Credentials = {
    email: Email;
    password: Password;
}

[<CLIMutable>]
type Account = {
    id: AccountId;
    credentials: Credentials;
}