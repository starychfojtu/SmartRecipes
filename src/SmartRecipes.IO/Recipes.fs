namespace SmartRecipes.IO

open SmartRecipes.Domain.Foodstuff
open SmartRecipes.Domain.Recipe
open SmartRecipes.Domain.SearchQuery

module Recipes =
    open FSharpPlus.Data
    open System
    open SmartRecipes.Domain.Account
    
    type IRecipesDao = 
        abstract member getByIds: Guid seq -> Recipe seq
        abstract member search: SearchQuery -> Recipe seq
        abstract member getByAccount: AccountId -> Recipe seq
        abstract member add: Recipe -> Recipe
        abstract member getRecommendationCandidates: FoodstuffId seq -> Recipe seq
            
    let getByIds<'e when 'e :> IRecipesDao> ids = Reader(fun (e : 'e) -> e.getByIds ids)
    let getByAccount<'e when 'e :> IRecipesDao> accountId = Reader(fun (e : 'e) -> e.getByAccount accountId)
    let search<'e when 'e :> IRecipesDao> query = Reader(fun (e : 'e) -> e.search query)
    let add<'e when 'e :> IRecipesDao> recipe = Reader(fun (e : 'e) -> e.add recipe)
    let getRecommendationCandidates<'e when 'e :> IRecipesDao> foodstuffIds = Reader(fun (e : 'e) -> e.getRecommendationCandidates foodstuffIds)