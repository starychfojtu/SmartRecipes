namespace SmartRecipes.IO

open FSharpPlus.Data
open Infrastructure

module IO =
    type IO<'a, 'env, 'error> = ReaderT<'env, Result<'a, 'error>>

    let success = ReaderT.hoistOk
    let fromResult = ReaderT.id

