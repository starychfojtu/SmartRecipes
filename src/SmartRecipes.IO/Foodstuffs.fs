namespace SmartRecipes.IO

module Foodstuffs =
    open System
    open FSharpPlus.Data
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SearchQuery
    
    type IFoodstuffDao = 
        abstract member getByIds: seq<Guid> -> seq<Foodstuff>
        abstract member search: SearchQuery -> seq<Foodstuff>
        abstract member add: Foodstuff -> Foodstuff
            
    let getByIds<'e when 'e :> IFoodstuffDao> ids = Reader(fun (e : 'e) -> e.getByIds ids)
    let search<'e when 'e :> IFoodstuffDao> query = Reader(fun (e : 'e) -> e.search query)
    let add<'e when 'e :> IFoodstuffDao> foodstuff = Reader(fun (e : 'e) -> e.add foodstuff)