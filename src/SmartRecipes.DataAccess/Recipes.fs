namespace SmartRecipes.DataAccess
open SmartRecipes.Domain.NaturalNumber
open SmartRecipes.Domain.Recipe

module Recipes =
    open FSharpPlus.Data
    open System
    open SmartRecipes.Domain
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
        
    let private nonEmptyStringOptionToModel =
        Option.ofObj >> Option.map (create >> Option.get)
        
    let private nonEmptyStringOptionToDb =
        Option.map (fun (s: NonEmptyString) -> s.Value) >> Option.toObj
    
    let private ingredientToModel (dbIngredient: DbIngredient): Ingredient = {
        FoodstuffId = FoodstuffId(dbIngredient.foodstuffId)
        Amount = Option.map amountToModel dbIngredient.amount
        Comment = nonEmptyStringOptionToModel dbIngredient.comment
        DisplayLine = nonEmptyStringOptionToModel dbIngredient.displayLine
    }
    
    let private ingredientToDb (ingredient: Ingredient): DbIngredient = {
        foodstuffId = ingredient.FoodstuffId.value
        amount = Option.map amountToDb ingredient.Amount
        comment = nonEmptyStringOptionToDb ingredient.Comment
        displayLine = nonEmptyStringOptionToDb ingredient.DisplayLine
    }
    
    let private difficultyToModel = function
        | DbDifficulty.Easy -> Difficulty.Easy
        | DbDifficulty.Normal -> Difficulty.Normal
        | DbDifficulty.Hard -> Difficulty.Hard
        | _ -> failwith "Invalid difficulty."
        
    let private difficultyToDb = function
        | Difficulty.Easy -> DbDifficulty.Easy
        | Difficulty.Normal -> DbDifficulty.Normal
        | Difficulty.Hard -> DbDifficulty.Hard
        
    let private cookingTimeToModel (time: DbCookingTime): CookingTime = {
        Text = NonEmptyString.create time.Text |> Option.get
    }
    
    let private cookingTimeToDb (time: CookingTime): DbCookingTime = {
        Text = time.Text.Value
    }
    
    let private nutritionInfoToModel (info: DbNutritionInfo): NutritionInfo = {
        Grams = NaturalNumber.create info.Grams |> Option.get
        Percents = Option.map (NaturalNumber.create >> Option.get) info.Percents
    }
    
    let private nutritionInfoToDb (info: NutritionInfo): DbNutritionInfo = {
        Grams = int info.Grams.Value
        Percents = Option.map (fun (n: NaturalNumber) -> int n.Value) info.Percents
    }
    
    let private nutritionPerServingToModel (nutrition: DbNutritionPerServing): NutritionPerServing = {
        Calories = Option.map (NaturalNumber.create >> Option.get) nutrition.Calories
        Fat = Option.map nutritionInfoToModel nutrition.Fat
        SaturatedFat = Option.map nutritionInfoToModel nutrition.SaturatedFat
        Sugars = Option.map nutritionInfoToModel nutrition.Sugars
        Protein = Option.map nutritionInfoToModel nutrition.Protein
        Carbs = Option.map nutritionInfoToModel nutrition.Carbs
    }
    
    let private nutritionPerServingToDb (nutrition: NutritionPerServing): DbNutritionPerServing = {
        Calories = Option.map (fun (n: NaturalNumber) -> int n.Value) nutrition.Calories
        Fat = Option.map nutritionInfoToDb nutrition.Fat
        SaturatedFat = Option.map nutritionInfoToDb nutrition.SaturatedFat
        Sugars = Option.map nutritionInfoToDb nutrition.Sugars
        Protein = Option.map nutritionInfoToDb nutrition.Protein
        Carbs = Option.map nutritionInfoToDb nutrition.Carbs
    }
    
    let private toModel (dbRecipe: DbRecipe): Recipe = {
        Id = RecipeId dbRecipe.Id
        Name = Utils.toNonEmptyStringModel dbRecipe.Name
        CreatorId = AccountId dbRecipe.CreatorId
        PersonCount = Utils.toNaturalNumberModel dbRecipe.PersonCount
        ImageUrl = dbRecipe.ImageUrl |> Option.ofObj |> Option.map Uri
        Url = dbRecipe.Url |> Option.ofObj |> Option.map Uri
        Description = nonEmptyStringOptionToModel dbRecipe.Description
        Ingredients = Seq.map ingredientToModel dbRecipe.Ingredients |> (mkNonEmptyList >> forceSucces)
        Difficulty = Option.map difficultyToModel dbRecipe.Difficulty
        CookingTime = Option.map cookingTimeToModel dbRecipe.CookingTime
        Tags = Seq.map (NonEmptyString.create >> Option.get >> RecipeTag) dbRecipe.Tags
        Rating = Option.map (Rating.create >> Option.get) dbRecipe.Rating
        NutritionPerServing = nutritionPerServingToModel dbRecipe.NutritionPerServing
    }
    
    let private toDb (recipe: Recipe): DbRecipe = {
        Id = match recipe.Id with RecipeId id -> id
        Name = recipe.Name.Value
        CreatorId = match recipe.CreatorId with AccountId id -> id
        PersonCount = Convert.ToInt32 recipe.PersonCount
        ImageUrl = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.ImageUrl |> Option.toObj
        Url = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.Url |> Option.toObj
        Description = nonEmptyStringOptionToDb recipe.Description
        Ingredients = NonEmptyList.map ingredientToDb recipe.Ingredients |> NonEmptyList.toSeq
        Difficulty = Option.map difficultyToDb recipe.Difficulty
        CookingTime = Option.map cookingTimeToDb recipe.CookingTime
        Tags = Seq.map (fun (t: RecipeTag) -> t.Value.Value) recipe.Tags
        Rating = Option.map (fun (r: Rating) -> int r.Value.Value) recipe.Rating
        NutritionPerServing = nutritionPerServingToDb recipe.NutritionPerServing
    }
    
    let private getByIds ids =
        collection().AsQueryable()
        |> Seq.filter (fun r -> Seq.contains r.Id ids)
        |> Seq.map toModel
        
    let private getById id =
        getByIds [id] |> Seq.tryHead
    
    let private getByAccount (AccountId accountId) =
        collection().AsQueryable()
        |> Seq.filter (fun r -> r.CreatorId = accountId)
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