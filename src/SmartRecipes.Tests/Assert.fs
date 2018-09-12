module Tests.Assert
    open System
    
    let fail message =
        raise (InvalidOperationException(message))
    
    let IsOk = function 
        | Ok _ -> ()
        | Error _ -> fail "Result is Error, expected Ok"
        
    let IsError = function 
        | Ok _ -> fail "Result is Ok, expected Error"
        | Error _ -> ()
        
    let IsErrorAnd otherAssert = function 
        | Ok _ -> fail "Result is Ok, expected Error"
        | Error e -> otherAssert e