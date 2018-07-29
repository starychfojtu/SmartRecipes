module Infrastructure.Reader
    open FSharpPlus
    open FSharpPlus.Data
    open Infrastructure
    
    let bindResult (binder: 'b -> Reader<'a, Result<'d, 'c>>) (reader: Reader<'a, Result<'b, 'e>>) =
        Reader(fun (a: 'a) ->
            Reader.run reader a
            |> Result.bind (fun d ->  Reader.run (binder d) a)
        )
        
    let private revertBindResult reader binder = bindResult binder reader
    let ( >>=! ) = revertBindResult
    
    let execute a reader =
        Reader.run reader a
        
    let id a = 
        Reader(fun (b) -> a)