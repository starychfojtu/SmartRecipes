module DataAccess.Recipes
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Models.Recipe
    open Models.Account
    open DataAccess.Model
    open Models
    open Models
    open Models.Foodstuff
    
    let private toModel (recipe: DbRecipeInfo): RecipeInfo = {
        id = RecipeId recipe.id
        name = Utils.toNonEmptyStringModel recipe.name
        creatorId = AccountId recipe.creatorId
        personCount = Utils.toNaturalNumberModel recipe.personCount
        imageUrl = Uri(recipe.imageUrl)
        description = recipe.description
    }
    
    let getByAccount (AccountId accountId) = Reader (fun (ctx: Context) ->
        ctx.Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
    )