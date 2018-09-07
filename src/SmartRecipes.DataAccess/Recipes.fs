module DataAccess.Recipes
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Domain.Recipe
    open Domain.Account
    open DataAccess.Model
    open Domain
    open Domain.NonEmptyString
    open Domain.Foodstuff
    open Utils
    
    type RecipesDao = {
        getByAccount: Guid -> seq<RecipeInfo>
    }
    
    let private toModel (recipe: DbRecipeInfo): RecipeInfo = {
        id = RecipeId recipe.id
        name = Utils.toNonEmptyStringModel recipe.name
        creatorId = AccountId recipe.creatorId
        personCount = Utils.toNaturalNumberModel recipe.personCount
        imageUrl = Uri(recipe.imageUrl)
        description = recipe.description |> Option.ofObj |> Option.map (mkNonEmptyString >> forceSucces)
    }
    
    let private getByAccount accountId =
        createDbContext().Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
    
    let getDao () = {
        getByAccount = getByAccount
    }