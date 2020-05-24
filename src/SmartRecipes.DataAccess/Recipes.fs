namespace SmartRecipes.DataAccess
open System
open FSharpPlus
open Infrastructure
open MongoDB.Bson
open Npgsql.FSharp
open SmartRecipes.Domain.Foodstuff
open SmartRecipes.Domain.NaturalNumber
open SmartRecipes.Domain.Recipe
open SmartRecipes.Domain.SearchQuery
open Users

module Recipes =
    open FSharpPlus.Data
    open FSharpPlus.Lens
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open Model
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.Foodstuff.FoodstuffId
    open MongoDB.Driver
    open Utils
    open Infrastructure.NonEmptyList

    module Postgres =
        let readRecipe ingredients (read: RowReader): Recipe =
            let recipeId = RecipeId <| read.uuid "id"
            {
                Id = recipeId
                Name = toNonEmptyStringModel <| read.string "name"
                CreatorId = None
                PersonCount = toNaturalNumberModel <| read.int "personcount"
                ImageUrl = Option.map Uri <| read.stringOrNone "imageurl"
                Url = Option.map Uri <| read.stringOrNone "url"
                Description = Option.map toNonEmptyStringModel <| read.stringOrNone "description"
                Ingredients = Map.find recipeId ingredients
                Difficulty = None
                CookingTime = read.stringOrNone "cookingtime" |> Option.map (fun t -> { Text = toNonEmptyStringModel t })
                Tags = Seq.empty
                Rating = None
                NutritionPerServing = {
                    Calories = None
                    Fat = None
                    SaturatedFat = None
                    Sugars = None
                    Salt = None
                    Protein = None
                    Carbs = None
                    Fibre = None
                }
            }

        let readIngredient (read: RowReader): RecipeId * Ingredient =
            let ingredient = {
                FoodstuffId = FoodstuffId <| read.uuid "foodstuffid"
                Amount =
                    match (read.doubleOrNone "amount", read.stringOrNone "unit") with
                    | Some a, Some u -> Some { unit = MetricUnit.MetricUnit <| toNonEmptyStringModel u; value = toNonNegativeFloatModel a }
                    | Some _, _ -> failwith "db error"
                    | _, Some _ -> failwith "db error"
                    | None, None -> None
                DisplayLine = toNonEmptyStringModel <| read.string "displayline"
                Comment = Option.map toNonEmptyStringModel <| read.stringOrNone "comment"
            }
            RecipeId <| read.uuid "recipeid", ingredient

//        let getByIds conn ids =
//            conn
//            |> Sql.query "SELECT * From dbo.recipe WHERE id = ANY(@ids)"
//            |> Sql.parameters [ "ids", Sql.uuidArray <| Seq.toArray ids ]
//            |> Sql.execute readRecipe
//            |> function | Ok l -> l | Error e -> failwith e.Message
//
//        let search conn (query: SearchQuery) =
//            conn
//            |> Sql.query "SELECT * From dbo.recipe WHERE name LIKE @query"
//            |> Sql.parameters [ "query", Sql.string ("%" + query.Value + "%") ]
//            |> Sql.execute readRecipe
//            |> function | Ok l -> l | Error e -> failwith e.Message

        let getAllIngredients conn =
            conn
            |> Sql.query "SELECT * From dbo.ingredient"
            |> Sql.execute readIngredient
            |> function | Ok l -> l | Error e -> failwith e.Message

        let getAllRecipes conn =
            let ingredients =
                getAllIngredients conn
                |> List.groupBy (fun (recipeId, _) -> recipeId)
                |> List.map (fun (k, v) -> (k, v |> List.map (fun (_, ingredient) -> ingredient) |> mkNonEmptyList |> Validation.forceSucces))
                |> Map.ofList

            conn
            |> Sql.query "SELECT * From dbo.recipe LIMIT 5000"
            |> Sql.execute (readRecipe ingredients)
            |> function | Ok l -> l | Error e -> failwith e.Message

    module Mongo =

        let private collection = Mongo.getCollection<DbRecipe> ()

        let private nonEmptyStringOptionToModel =
            Option.ofObj >> Option.map (create >> Option.get)

        let private nonEmptyStringOptionToDb =
            Option.map (fun (s: NonEmptyString) -> s.Value) >> Option.toObj

        let private ingredientToModel (dbIngredient: DbIngredient): Ingredient = {
            FoodstuffId = FoodstuffId(dbIngredient.foodstuffId)
            Amount = Option.ofObj dbIngredient.amount |> Option.map amountToModel
            Comment = nonEmptyStringOptionToModel dbIngredient.comment
            DisplayLine = toNonEmptyStringModel dbIngredient.displayLine
        }

        let private ingredientToDb (ingredient: Ingredient): DbIngredient = {
            foodstuffId = ingredient.FoodstuffId.value
            amount = Option.map amountToDb ingredient.Amount |> Option.toObj
            comment = nonEmptyStringOptionToDb ingredient.Comment
            displayLine = ingredient.DisplayLine.Value
        }

        let private difficultyToModel = function
            | DbDifficulty.Easy -> Some Difficulty.Easy
            | DbDifficulty.Normal -> Some Difficulty.Normal
            | DbDifficulty.Hard -> Some Difficulty.Hard
            | DbDifficulty.Unspecified -> None
            | _ -> failwith "Invalid difficulty."

        let private difficultyToDb = function
            | Some Difficulty.Easy -> DbDifficulty.Easy
            | Some Difficulty.Normal -> DbDifficulty.Normal
            | Some Difficulty.Hard -> DbDifficulty.Hard
            | None -> DbDifficulty.Unspecified

        let private cookingTimeToModel (time: DbCookingTime): CookingTime = {
            Text = NonEmptyString.create time.text |> Option.get
        }

        let private cookingTimeToDb (time: CookingTime) =
            DbCookingTime(time.Text.Value)

        let private nutritionInfoToModel (info: DbNutritionInfo): NutritionInfo = {
            Grams = NonNegativeFloat.create info.grams |> Option.get
            Percents = Option.ofNullable info.percents |> Option.map (NaturalNumber.create >> Option.get)
        }

        let private nutritionInfoToDb (info: NutritionInfo) =
            DbNutritionInfo(
                info.Grams.Value,
                Option.map (fun (n: NaturalNumber) -> int n.Value) info.Percents |> Option.toNullable
            )

        let private nutritionPerServingToModel (nutrition: DbNutritionPerServing): NutritionPerServing = {
            Calories = Option.ofNullable nutrition.Calories |> Option.map (NaturalNumber.create >> Option.get)
            Fat = Option.ofObj nutrition.Fat |> Option.map nutritionInfoToModel
            SaturatedFat = Option.ofObj nutrition.SaturatedFat |> Option.map nutritionInfoToModel
            Sugars = Option.ofObj nutrition.Sugars |> Option.map nutritionInfoToModel
            Salt = Option.ofObj nutrition.Salt |> Option.map nutritionInfoToModel
            Protein = Option.ofObj nutrition.Protein |> Option.map nutritionInfoToModel
            Carbs = Option.ofObj  nutrition.Carbs |> Option.map nutritionInfoToModel
            Fibre = Option.ofObj  nutrition.Fibre |> Option.map nutritionInfoToModel
        }

        let private nutritionPerServingToDb (nutrition: NutritionPerServing): DbNutritionPerServing = {
            Calories = Option.map (fun (n: NaturalNumber) -> int n.Value) nutrition.Calories |> Option.toNullable
            Fat = Option.map nutritionInfoToDb nutrition.Fat |> Option.toObj
            SaturatedFat = Option.map nutritionInfoToDb nutrition.SaturatedFat |> Option.toObj
            Sugars = Option.map nutritionInfoToDb nutrition.Sugars |> Option.toObj
            Salt = Option.map nutritionInfoToDb nutrition.Salt |> Option.toObj
            Protein = Option.map nutritionInfoToDb nutrition.Protein |> Option.toObj
            Carbs = Option.map nutritionInfoToDb nutrition.Carbs |> Option.toObj
            Fibre = Option.map nutritionInfoToDb nutrition.Fibre |> Option.toObj
        }

        let private toModel (dbRecipe: DbRecipe): Recipe = {
            Id = RecipeId dbRecipe.Id
            Name = Utils.toNonEmptyStringModel dbRecipe.Name
            CreatorId = Option.map AccountId <| Option.ofNullable dbRecipe.CreatorId
            PersonCount = Utils.toNaturalNumberModel dbRecipe.PersonCount
            ImageUrl = dbRecipe.ImageUrl |> Option.ofObj |> Option.map Uri
            Url = dbRecipe.Url |> Option.ofObj |> Option.map Uri
            Description = nonEmptyStringOptionToModel dbRecipe.Description
            Ingredients = Seq.map ingredientToModel dbRecipe.Ingredients |> (mkNonEmptyList >> forceSucces)
            Difficulty = difficultyToModel dbRecipe.Difficulty
            CookingTime = Option.ofObj dbRecipe.CookingTime |> Option.map cookingTimeToModel
            Tags = Seq.map (NonEmptyString.create >> Option.get >> RecipeTag) dbRecipe.Tags
            Rating = Option.ofNullable dbRecipe.Rating |> Option.map (Rating.create >> Option.get)
            NutritionPerServing = nutritionPerServingToModel dbRecipe.NutritionPerServing
        }

        let private toDb (recipe: Recipe): DbRecipe = {
            Id = match recipe.Id with RecipeId id -> id
            Name = recipe.Name.Value
            CreatorId = match recipe.CreatorId with Some (AccountId id) -> Nullable(id) | None -> Nullable()
            PersonCount = int recipe.PersonCount.Value
            ImageUrl = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.ImageUrl |> Option.toObj
            Url = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.Url |> Option.toObj
            Description = nonEmptyStringOptionToDb recipe.Description
            Ingredients = NonEmptyList.map ingredientToDb recipe.Ingredients |> NonEmptyList.toSeq
            Difficulty = difficultyToDb recipe.Difficulty
            CookingTime = Option.map cookingTimeToDb recipe.CookingTime |> Option.toObj
            Tags = Seq.map (fun (t: RecipeTag) -> t.Value.Value) recipe.Tags
            Rating = Option.map (fun (r: Rating) -> int r.Value.Value) recipe.Rating |> Option.toNullable
            NutritionPerServing = nutritionPerServingToDb recipe.NutritionPerServing
        }

        let getByIds ids =
            collection.AsQueryable()
            |> Seq.filter (fun r -> Seq.contains r.Id ids)
            |> Seq.map toModel

        let search (query: SearchQuery) =
            let regex = BsonRegularExpression(query.Value)
            let filter = Builders<DbRecipe>.Filter.Regex((fun r -> r.Name :> obj), regex)
            collection.FindSync(filter).ToEnumerable() |> Seq.map toModel

        let getByAccount (AccountId accountId) =
            collection.AsQueryable()
            |> Seq.filter (fun r -> r.CreatorId.Value = accountId)
            |> Seq.map toModel

        let add recipe =
            toDb recipe |> collection.InsertOne |> ignore
            recipe

        let getRecommendedationCandidates foodstuffIds =
            let foodstuffIdValues = Seq.map (view _value) foodstuffIds
            let ingredientFilter = Builders<DbIngredient>.Filter.In((fun i -> i.foodstuffId), foodstuffIdValues);
            let filter = Builders<DbRecipe>.Filter.ElemMatch((fun r -> r.Ingredients), ingredientFilter);
            collection.FindSync(filter).ToEnumerable() |> Seq.map toModel