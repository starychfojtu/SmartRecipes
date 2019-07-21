namespace Infrastructure

module ReaderT =
    open FSharpPlus.Data

    let id a =
        ReaderT(fun _ -> a)
    
    let execute a reader =
        ReaderT.run reader a
        
    let mapDirect f reader = ReaderT(fun env ->
        execute env reader |> f)
    
    let mapError<'e, 'a, 'b, 's> f (reader: ReaderT<'e, Result<'s, 'a>>): ReaderT<'e, Result<'s, 'b>> =
        mapDirect (Result.mapError f) reader
        
    let fromReader r = ReaderT(fun env -> Reader.run r env)
    
    let hoistOk r = Reader.map Ok r |> fromReader