namespace SmartRecipes.Domain

module Email =
    open System.Net.Mail
    open FSharpPlus.Data

    type EmailError =
        | Invalid
                
    let mkEmail s =
        try
            Success (new MailAddress(s))
        with
        | ex -> Failure [ Invalid ]