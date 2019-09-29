namespace SmartRecipes.UseCases

open System
open SmartRecipes.IO

module DateTimeProvider =
    
    type IDateTimeProvider =
        abstract member nowUtc: unit -> DateTime
        
    let nowUtc<'a when 'a :> IDateTimeProvider> =  IO.operation (fun (provider: 'a) -> provider.nowUtc ())