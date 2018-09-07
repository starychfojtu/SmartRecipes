module DataAccess.Recipes
    open DataAccess
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Domain.Recipe
    open Domain.Account
    open DataAccess.Model
    open Domain
    open Domain
    open Domain.NonEmptyString
    open Domain.Foodstuff
    open FSharpPlus.Data
    open System
    open System
    open Utils
    
    type RecipesDao = {
        getByAccount: Guid -> seq<RecipeInfo>
        add: Recipe -> Recipe
    }
    
    let private infoToModel (recipe: DbRecipeInfo): RecipeInfo = {
        id = RecipeId recipe.id
        name = Utils.toNonEmptyStringModel recipe.name
        creatorId = AccountId recipe.creatorId
        personCount = Utils.toNaturalNumberModel recipe.personCount
        imageUrl = Uri(recipe.imageUrl)
        description = recipe.description |> Option.ofObj |> Option.map (mkNonEmptyString >> forceSucces)
    }
    
    let private infoToDb (info: RecipeInfo): DbRecipeInfo = {
        id = match info.id with RecipeId id -> id
        name = info.name.value
        creatorId = match info.creatorId with AccountId id -> id
        personCount = Convert.ToInt32 info.personCount
        imageUrl = info.imageUrl.AbsolutePath
        description = info.description |> Option.map (fun d -> d.value) |> Option.toObj
    }
    
    let private ingredientToDb (ingredient: Ingredient): DbIngredient = {
        id = Guid.NewGuid ()
        recipeId = match ingredient.recipeId with RecipeId id -> id
        foodstuffId = match ingredient.foodstuffId with FoodstuffId id -> id
        amount = Foodstuffs.amountToDb ingredient.amount
    }
    
    let private toDb recipe =
        let dbInfo = infoToDb recipe.info
        let dbIngretients = recipe.ingredients |> NonEmptyList.map ingredientToDb |> NonEmptyList.toSeq
        (dbInfo, dbIngretients)
    
    let private getByAccount accountId =
        createDbContext().Recipes
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map infoToModel
        
    let private add recipe =
        let context = createDbContext()
        let (info, ingredients) = toDb recipe
        context.Add (info) |> ignore
        context.Add (ingredients) |> ignore
        context.SaveChanges |> ignore
        recipe
    
    let getDao () = {
        getByAccount = getByAccount
        add = add
    }