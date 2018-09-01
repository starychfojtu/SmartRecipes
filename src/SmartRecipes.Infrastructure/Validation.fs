module Infrastructure.Validation
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open System

    let mapFailure mapper =
        bimap (fun e -> mapper e) (fun s -> s)
        
    let forceSucces = function 
        | Success s -> s
        | Failure f -> raise (InvalidOperationException())