module UseCases.Users
    open System.Net.Mail
    open Business
    open Business
    open DataAccess
    open Database.Context
    open Database.Model
    open FSharpPlus
    open FSharpPlus.Data
    
    let signUp email password = 
        let context = createDbContext ()
        let getUserByEmail = (fun email -> Users.getUserByEmail email |> Reader.run <| context) // TODO: refactor
        let result = Users.signUp email password getUserByEmail
        Result.map (fun a -> Users.add context a) result
        
    let signIn email password =
        let authenticate = monad { 
            let! user = Users.getUserByEmail email
            let signedIn = Option.map (fun u -> Users.signIn u password) user
            return match signedIn with
            | Some(true) -> true
            | _ -> false
        }
        Reader.run authenticate <| createDbContext ()
        
