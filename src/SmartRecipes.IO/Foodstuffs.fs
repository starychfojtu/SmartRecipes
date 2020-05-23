namespace SmartRecipes.IO

module Foodstuffs =
    open System
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SearchQuery

    type IFoodstuffDao =
        abstract member getByIds: seq<Guid> -> seq<Foodstuff>
        abstract member search: SearchQuery -> seq<Foodstuff>
        abstract member add: Foodstuff -> Foodstuff
        abstract member getVectors: unit -> Map<FoodstuffId, float[]>

    let getByIds<'e when 'e :> IFoodstuffDao> ids = IO.operation (fun (e : 'e) -> e.getByIds ids)
    let search<'e when 'e :> IFoodstuffDao> query = IO.operation (fun (e : 'e) -> e.search query)
    let add<'e when 'e :> IFoodstuffDao> foodstuff = IO.operation (fun (e : 'e) -> e.add foodstuff)
    let vectors<'e when 'e :> IFoodstuffDao> = IO.operation (fun (e : 'e) -> e.getVectors ())