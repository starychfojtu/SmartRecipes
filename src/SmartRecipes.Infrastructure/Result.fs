module Infrastracture.Result
    open System
    
    let forceOk = function 
        | Ok a -> a
        | Error e -> raise (InvalidOperationException("Result is error, but ok was forced."))