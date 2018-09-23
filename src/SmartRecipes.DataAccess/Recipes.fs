module DataAccess.Recipes
    open System.Collections.Generic
    open DataAccess
    open DataAccess
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Domain.Recipe
    open Domain.Account
    open DataAccess.Model
    open Domain
    open Domain.NonEmptyString
    open Domain.Foodstuff
    open MongoDB.Driver
    open FSharpPlus.Data
    open System
    open Utils
    open Infrastructure.NonEmptyList
    open Domain.NonNegativeFloat
    open FSharpPlus.Data
    
    type RecipesDao = {
        getByIds: seq<Guid> -> seq<Recipe>
        getById: Guid -> Recipe option
        getByAccount: AccountId -> seq<Recipe>
        add: Recipe -> Recipe
    }
    
    let private collection () = Database.getCollection<DbRecipe> ()
    
    let private ingredientToModel (dbIngredient: DbIngredient): Ingredient = {
        foodstuffId = FoodstuffId(dbIngredient.foodstuffId)
        amount = NonNegativeFloat.create dbIngredient.amount |> forceSucces
    }
    
    let private ingredientToDb (ingredient: Ingredient): DbIngredient = {
        foodstuffId = ingredient.foodstuffId.value
        amount = NonNegativeFloat.value ingredient.amount
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
        imageUrl = recipe.imageUrl.AbsoluteUri
        description = recipe.description |> Option.map (fun d -> d.value) |> Option.toObj
        ingredients = NonEmptyList.map ingredientToDb recipe.ingredients |> NonEmptyList.toSeq
    }
    
    let private getByIds ids =
        collection().AsQueryable()
        |> Seq.filter (fun r -> Seq.contains r.id ids)
        |> Seq.map toModel
        
    let private getById id =
        getByIds [id] |> Seq.tryHead
    
    let private getByAccount (AccountId accountId) =
        collection().AsQueryable()
        |> Seq.filter (fun r -> r.creatorId = accountId)
        |> Seq.map toModel
        
    let private add recipe =
        toDb recipe |> collection().InsertOne |> ignore
        recipe
    
    let getDao () = {
        getByIds = getByIds
        getById = getById
        getByAccount = getByAccount
        add = add
    }