module Infrastracture.Result
    open System
    
    let forceOk = function 
        | Ok a -> a
        | Error e -> raise (InvalidOperationException("Result is error, but ok was forced."))
        
    let bimap f g =
        Result.map f >> Result.mapError g
        