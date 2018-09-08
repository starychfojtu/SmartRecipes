module DataAccess.Recipes
    open System.Collections.Generic
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
    open Domain.FoodstuffAmount
    open FSharpPlus.Data
    open System
    open Utils
    open Infrastructure.NonEmptyList
    open Domain.NonNegativeFloat
    open FSharpPlus.Data
    open Microsoft.EntityFrameworkCore
    
    type RecipesDao = {
        getByAccount: Guid -> seq<Recipe>
        add: Recipe -> Recipe
    }
    
    let private ingredientToModel (dbIngredient: DbIngredient) = {
        foodstuff = Foodstuffs.toModel dbIngredient.foodstuff
        amount = mkNonNegativeFloat dbIngredient.amount |> forceSucces
    }
    
    let private ingredientToDb (recipeId: RecipeId) ingredient = {
        id = recipeId.value.ToString () + ingredient.foodstuff.id.ToString ()
        recipeId = recipeId.value
        foodstuffId = ingredient.foodstuff.id.value
        foodstuff = Foodstuffs.toDb ingredient.foodstuff
        amount = ingredient.amount.value
    }
    
    let private toModel (dbRecipe: DbRecipe): Recipe = {
        id = RecipeId dbRecipe.id
        name = Utils.toNonEmptyStringModel dbRecipe.name
        creatorId = AccountId dbRecipe.creatorId
        personCount = Utils.toNaturalNumberModel dbRecipe.personCount
        imageUrl = Uri(dbRecipe.imageUrl)
        description = dbRecipe.description |> Option.ofObj |> Option.map (mkNonEmptyString >> forceSucces)
        ingredients = Seq.map ingredientToModel dbRecipe.ingredients |> (mkNonEmptyList >> forceSucces)
    }
    
    let private toDb (recipe: Recipe): DbRecipe = {
        id = match recipe.id with RecipeId id -> id
        name = recipe.name.value
        creatorId = match recipe.creatorId with AccountId id -> id
        personCount = Convert.ToInt32 recipe.personCount
        imageUrl = recipe.imageUrl.AbsolutePath
        description = recipe.description |> Option.map (fun d -> d.value) |> Option.toObj
        ingredients = NonEmptyList.map (ingredientToDb recipe.id) recipe.ingredients |> NonEmptyList.toSeq
    }
    
    // TODO: Add Include
    let private getByAccount accountId =
        createDbContext().Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
        
    let private add recipe =
        let context = createDbContext()
        context.Add (toDb recipe) |> ignore
        context.SaveChanges () |> ignore
        recipe
    
    let getDao () = {
        getByAccount = getByAccount
        add = add
    }