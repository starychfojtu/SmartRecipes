namespace Infrastructure

module ReaderT =
    open FSharpPlus.Data

    let id a =
        ReaderT(fun _ -> a)
    
    let execute a reader =
        ReaderT.run reader a
        
    let private mapMonad f reader = ReaderT(fun env ->
        execute env reader |> f)
    
    let mapError<'e, 'a, 'b, 's> f (reader: ReaderT<'e, Result<'s, 'a>>): ReaderT<'e, Result<'s, 'b>> =
        mapMonad (Result.mapError f) reader