namespace SmartRecipes.DataAccess

module Recipes =
    open FSharpPlus.Data
    open System
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.Account
    open Model
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.Foodstuff
    open MongoDB.Driver
    open Utils
    open Infrastructure.NonEmptyList
    
    type RecipesDao = {
        getByIds: seq<Guid> -> seq<Recipe>
        getById: Guid -> Recipe option
        getByAccount: AccountId -> seq<Recipe>
        add: Recipe -> Recipe
    }
    
    let private collection () = Database.getCollection<DbRecipe> ()
    
    let private ingredientToModel (dbIngredient: DbIngredient): Ingredient = {
        FoodstuffId = FoodstuffId(dbIngredient.foodstuffId)
        Amount = amountToModel dbIngredient.amount
    }
    
    let private ingredientToDb (ingredient: Ingredient): DbIngredient = {
        foodstuffId = ingredient.FoodstuffId.value
        amount = amountToDb ingredient.Amount
    }
    
    let private toModel (dbRecipe: DbRecipe): Recipe = {
        Id = RecipeId dbRecipe.id
        Name = Utils.toNonEmptyStringModel dbRecipe.name
        CreatorId = AccountId dbRecipe.creatorId
        PersonCount = Utils.toNaturalNumberModel dbRecipe.personCount
        ImageUrl = Uri(dbRecipe.imageUrl)
        Description = dbRecipe.description |> Option.ofObj |> Option.map (create >> forceSucces)
        Ingredients = Seq.map ingredientToModel dbRecipe.ingredients |> (mkNonEmptyList >> forceSucces)
    }
    
    let private toDb (recipe: Recipe): DbRecipe = {
        id = match recipe.Id with RecipeId id -> id
        name = recipe.Name.Value
        creatorId = match recipe.CreatorId with AccountId id -> id
        personCount = Convert.ToInt32 recipe.PersonCount
        imageUrl = recipe.ImageUrl.AbsoluteUri
        description = recipe.Description |> Option.map (fun d -> d.Value) |> Option.toObj
        ingredients = NonEmptyList.map ingredientToDb recipe.Ingredients |> NonEmptyList.toSeq
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
    
    let dao = {
        getByIds = getByIds
        getById = getById
        getByAccount = getByAccount
        add = add
    }