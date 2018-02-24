module Account

open System
open Recipe

type Email = string
type Password = string

type SignInInfo = {
    email: Email;
    password: Password;
}

type Account = {
    id: Guid;
    signInInfo: SignInInfo;
    recipes: Recipes;
}