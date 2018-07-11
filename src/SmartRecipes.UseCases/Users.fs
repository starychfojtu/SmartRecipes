module UseCases.Users
    open System.Net.Mail
    open Business
    open Database.Context
    open Database.Model
    open Models.User
    open FSharpPlus.Data
    
    // TODO: move this to data access
    let toModel (dbAccount: Database.Model.Account): Models.User.Account option =
        match mkCredentials dbAccount.email dbAccount.password with 
        | Success c -> Some { id = AccountId dbAccount.id; credentials = c }
        | Failure _ -> None
        
    // TODO: move this to data access
    let getUserByEmail (context: Context) (email: MailAddress) =
        context.Accounts 
        |> Seq.filter (fun a -> a.email = email.Address)
        |> Seq.tryHead
        |> Option.bind (fun a -> toModel a)
        
    
    let signUp email password = 
        let context = createDbContext ()
        let result = Users.signUp email password <| getUserByEmail context
        match result with
        | Ok a ->
            context.Add(a) |> ignore
            context.SaveChanges() |> ignore
            Ok a
        | Error e -> Error e  
