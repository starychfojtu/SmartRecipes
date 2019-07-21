namespace SmartRecipes.UseCases

open System
open FSharpPlus.Data

module DateTimeProvider =
    
    type IDateTimeProvider =
        abstract member nowUtc: unit -> DateTime
        
    let nowUtc<'a when 'a :> IDateTimeProvider> = Reader(fun (provider: 'a) -> provider.nowUtc ())