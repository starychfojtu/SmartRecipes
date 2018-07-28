module DataAccess.Recipes
    open DataAccess.Context
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Models.Recipe
    open Models.Account
    open DataAccess.Model
    open Models.Foodstuff
    
    let private toModel (recipe: DbRecipe): Recipe = {
        id = RecipeId recipe.id;
        name = recipe.name;
        creatorId = AccountId recipe.creatorId
    }
    
    let getByAccount (AccountId accountId) = Reader (fun (ctx: Context) ->
        ctx.Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
    )