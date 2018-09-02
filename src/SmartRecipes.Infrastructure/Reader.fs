module Infrastructure.Reader
    open FSharpPlus
    open FSharpPlus.Data
    open Infrastructure
    
    let bindResult (binder: 'b -> Reader<'a, Result<'d, 'e>>) (reader: Reader<'a, Result<'b, 'e>>) =
        Reader(fun (a: 'a) ->
            Reader.run reader a
            |> Result.bind (fun d ->  Reader.run (binder d) a)
        )
        
    let revertBindResult reader binder = bindResult binder reader
    let ( >>=! ) = revertBindResult
    
    let execute a reader =
        Reader.run reader a
        
    let id a = 
        Reader(fun b -> a)
        
    let mapEnviroment map revert reader = 
        
    let a = Reader(fun (a: int) -> a)
    let b = Reader(fun (a: string) -> a)
    
    let c = Reader.local (fun t -> (t, t)) a