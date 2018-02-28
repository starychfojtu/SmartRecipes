namespace SmartRecipes.Models

open System

type Email = string
type Password = string

type SignInInfo = {
    email: Email;
    password: Password;
}

type Account = {
    id: Guid;
    signInInfo: SignInInfo;
}