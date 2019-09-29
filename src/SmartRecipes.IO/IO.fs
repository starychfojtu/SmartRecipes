namespace SmartRecipes.IO

open FSharpPlus.Data
open Infrastructure
open Infrastructure.Void

module IO =
    type IO<'a, 'env, 'error> = ReaderT<'env, Result<'a, 'error>>
    type IO<'a, 'env> = Reader<'env, 'a>

    let success r: IO<'a, 'env, 'error> = ReaderT.hoistOk r
    
    let toEIO (f: 'a -> Result<'b, 'e>) (io: IO<'a, 'env>): IO<'b, 'env, 'e> = ReaderT.mapDirect f (ReaderT.fromReader io)
    
    let toSuccessEIO (io: IO<'a, 'env>): IO<'a, 'env, 'e>  = toEIO Ok io
    
    let fromResult r: IO<'a, 'env, 'error> = ReaderT.id r 
    
    let operation f: IO<'a, 'env> = Reader(f)
    
    let map (f: 'a -> 'b) (io: IO<'a, 'env, 'e>): IO<'b, 'env, 'e> = ReaderT.map f io
    
    let mapError (f: 'e1 -> 'e2) (io: IO<'a, 'env, 'e1>): IO<'a, 'env, 'e2> = ReaderT.mapError f io
    
    let bindError (f: Result<'a, 'e1> -> Result<'b, 'e2>) (io: IO<'a, 'env, 'e1>): IO<'b, 'env, 'e2> =
        ReaderT.mapDirect f io

