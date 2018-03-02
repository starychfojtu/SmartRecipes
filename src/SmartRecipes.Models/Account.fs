namespace SmartRecipes.Models

open System

type Email = string
type Password = string

[<CLIMutable>]
type SignInInfo = {
    email: Email;
    password: Password;
}

[<CLIMutable>]
type Account = {
    id: Guid;
    signInInfo: SignInInfo;
}