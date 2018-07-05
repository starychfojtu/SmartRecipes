namespace SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Models.Recipe
    open Models.User
    open Database.Context
    
    let private mapUnit (unit: Database.Model.MetricUnit): Models.Foodstuff.MetricUnit =
        match unit with
        | Database.Model.MetricUnit.Liter -> Models.Foodstuff.MetricUnit.Liter
        | Database.Model.MetricUnit.Gram -> Models.Foodstuff.MetricUnit.Gram
        | Database.Model.MetricUnit.Piece -> Models.Foodstuff.MetricUnit.Piece
        | _ -> Models.Foodstuff.MetricUnit.Piece
    
    let private map (recipe: Database.Model.Recipe): Recipe = {
        id = RecipeId recipe.id;
        name = recipe.name;
        creatorId = AccountId recipe.creatorId
    }
    
    let getAll: Reader<Context, seq<Recipe>> = Reader (fun ctx ->
        ctx.Recipes
        |> Seq.toList
        |> Seq.map map
    )