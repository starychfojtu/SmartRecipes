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
    
    type RecipesDao = {
        getByAccount: AccountId -> seq<RecipeInfo>
    }
    
    let private toModel (recipe: DbRecipeInfo): RecipeInfo = {
        id = RecipeId recipe.id
        name = Utils.toNonEmptyStringModel recipe.name
        creatorId = AccountId recipe.creatorId
        personCount = Utils.toNaturalNumberModel recipe.personCount
        imageUrl = Uri(recipe.imageUrl)
        description = recipe.description
    }
    
    let private getByAccount (AccountId accountId) =
        createDbContext().Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
    
    let getDao () = {
        getByAccount = getByAccount
    }