namespace SmartRecipes.Api

module Recipes =
    open Dto
    open Generic
    open SmartRecipes.DataAccess
    open System
    open Infrastructure
    open SmartRecipes.UseCases
    open FSharpPlus.Data
    open SmartRecipes.UseCases.Recipes
    open FSharpPlus
    open Infrastracture
    open Infrastructure.NonEmptyList
    open SmartRecipes.Domain.Recipe
    open AmountParameters
            
    module GetMyRecipes =
        type Response = {
            Recipes: RecipeDto list
        }
        
        let private serialize = 
            Result.bimap (fun rs -> { Recipes = Seq.map serializeRecipe rs |> Seq.toList }) (function GetMyRecipesError.Unauthorized -> "Unauthorized.")
        
        let private getMyRecipes accessToken _ = 
            Recipes.getMyRecipes accessToken
        
        let handler<'a> =
            authorizedGetHandler getMyRecipes serialize
        
    module GetByIds =
        [<CLIMutable>]
        type Parameters = {
            Ids: Guid list
        }
        
        type Response = {
            Recipes: RecipeDto list
        }
            
        let private serialize = 
            Result.bimap (fun fs -> { Recipes = Seq.map serializeRecipe fs |> Seq.toList }) (function Recipes.GetByIdsError.Unauthorized -> "Unauthorized.")
            
        let private getByIds accessToken parameters =
            Recipes.getByIds accessToken parameters.Ids

        let handler<'a> = 
            authorizedGetHandler getByIds serialize
        
    module Search =
        open SmartRecipes.Domain
        
        [<CLIMutable>]
        type Parameters = {
            query: string
        }
        
        type Response = {
            Recipes: RecipeDto list
        }
        
        type Error = 
            | BusinessError of Recipes.SearchError
            | QueryIsEmpty
            
        let private serializeSearchError = function 
            | BusinessError e ->
                match e with
                | Recipes.SearchError.Unauthorized -> "Unauthorized."
            | QueryIsEmpty -> "Query is empty."
            
        let private serializeSearch<'a, 'b> = 
            Result.bimap (fun rs -> { Recipes = Seq.map Dto.serializeRecipe rs |> Seq.toList }) serializeSearchError
            
        let private parseQuery parameters =
            Parse.nonEmptyString QueryIsEmpty parameters.query 
            |> Validation.map SearchQuery.create
            |> Validation.toResult 
            |> ReaderT.id
            
        let searchFoodstuffs accessToken query =
            Recipes.search accessToken query |> ReaderT.mapError BusinessError
            
        let search accessToken parameters = 
            parseQuery parameters
            >>= searchFoodstuffs accessToken
            
        let handler<'a> = 
            authorizedGetHandler search serializeSearch
    
    [<CLIMutable>]
    type IngredientParameter = {
        foodstuffId: Guid
        amount: AmountParameters option
        comment: string option
        displayLine: string option
    }
    
    [<CLIMutable>]
    type CookingTimeParameters = {
        Text: string
    }
    
    [<CLIMutable>]
    type NutritionInfoParameters = {
        Grams: float
        Percents: int option
    }
    
    [<CLIMutable>]
    type NutritionPerServingParameters = {
        Calories: int option
        Fat: NutritionInfoParameters option
        SaturatedFat: NutritionInfoParameters option
        Sugars: NutritionInfoParameters option
        Salt: NutritionInfoParameters option
        Protein: NutritionInfoParameters option
        Carbs: NutritionInfoParameters option
        Fibre: NutritionInfoParameters option
    }

    [<CLIMutable>]
    type CreateParameters = {
        Name: string
        PersonCount: int
        ImageUrl: string option
        Url: string option
        Description: string option
        Ingredients: IngredientParameter list
        Difficulty: string option
        CookingTime: CookingTimeParameters option
        Tags: string list
        Rating: int option
        Nutrition: NutritionPerServingParameters
    }
    
    type CreateResponse = {
        Recipe: RecipeDto
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | InvalidUrl of string
        | AmountError of AmountParameters.Error
        | MustContaintAtLeastOneIngredient
        | DescriptionIsProvidedButEmpty
        | DisplayLineOfIngredientIsProvidedButEmpty
        | CommentOfIngredientIsProvidedButEmpty
        | CookingTimeTextIsProvidedButEmpty
        | UnknownDifficulty
        | TagIsEmpty
        | InvalidRating
        | GramsMustBePositive
        | PercentsMustBePositive
        | CaloriesMustBePositive
        | BusinessError of Recipes.CreateError
    
    module IngredientParameters =
        
        let private parseIngredientAmount =
            Parse.option AmountParameters.parse >> Validation.mapFailure (List.map AmountError)

        let private parseOne parameter =
            Recipes.createIngredientParameters parameter.foodstuffId
            <!> parseIngredientAmount parameter.amount
            <*> Parse.nonEmptyStringOption [CommentOfIngredientIsProvidedButEmpty] parameter.comment
            <*> Parse.nonEmptyStringOption [DisplayLineOfIngredientIsProvidedButEmpty] parameter.displayLine
        
        let private toNonEmpty ingredients = 
            NonEmptyList.mkNonEmptyList ingredients 
            |> Validation.mapFailure (function SequenceIsEmpty -> [MustContaintAtLeastOneIngredient])
               
        let parse parameters =
            Seq.map parseOne parameters
            |> Validation.traverseSeq
            |> Validation.bind toNonEmpty
            
    module CookingTimeParameters =
        let parse parameters =
            CookingTime.create
            <!> Parse.nonEmptyString [CookingTimeTextIsProvidedButEmpty] parameters.Text
            
    module NutritionInfoParameters =
        let parse parameters =
            NutritionInfo.create
            <!> Parse.nonNegativeFloat [GramsMustBePositive] parameters.Grams
            <*> Parse.option (Parse.naturalNumber [PercentsMustBePositive]) parameters.Percents
            
    module NutritionPerServingParameters =
        let parse parameters =
            NutritionPerServing.create
            <!> Parse.option (Parse.naturalNumber [CaloriesMustBePositive]) parameters.Calories
            <*> Parse.option NutritionInfoParameters.parse parameters.Fat
            <*> Parse.option NutritionInfoParameters.parse parameters.SaturatedFat
            <*> Parse.option NutritionInfoParameters.parse parameters.Sugars
            <*> Parse.option NutritionInfoParameters.parse parameters.Salt
            <*> Parse.option NutritionInfoParameters.parse parameters.Protein
            <*> Parse.option NutritionInfoParameters.parse parameters.Carbs
            <*> Parse.option NutritionInfoParameters.parse parameters.Fibre
            
    module RecipeParameters =
        let private parseDifficulty s =
            match s with
            | "easy" -> Success Easy
            | "normal" -> Success Normal
            | "hard" -> Success Hard
            | _ -> Failure [UnknownDifficulty]
            
        let private parseRating =
            Rating.create >> Validation.ofOption [InvalidRating]
            
        let parse (parameters: CreateParameters) =
            createParameters
            <!> Parse.nonEmptyString [NameCannotBeEmpty] parameters.Name
            <*> Parse.naturalNumber [PersonCountMustBePositive] parameters.PersonCount
            <*> Parse.uriOption (fun m -> [InvalidImageUrl(m)]) parameters.ImageUrl
            <*> Parse.uriOption (fun m -> [InvalidUrl(m)]) parameters.Url
            <*> Parse.nonEmptyStringOption [DescriptionIsProvidedButEmpty] parameters.Description
            <*> IngredientParameters.parse parameters.Ingredients
            <*> Parse.option parseDifficulty parameters.Difficulty
            <*> Parse.option CookingTimeParameters.parse parameters.CookingTime
            <*> Parse.seqOf (Parse.nonEmptyString [TagIsEmpty]) parameters.Tags
            <*> Parse.option parseRating parameters.Rating
            <*> NutritionPerServingParameters.parse parameters.Nutrition
        
    let private createRecipe accessToken = 
        Recipes.create accessToken >> (ReaderT.mapError (fun e -> [BusinessError e]))
        
    let private serializeCreateIngredientError = function
        | DuplicateFoodstuffIngredient -> "Multiple ingredients with common foodstuff found."
        | FoodstuffNotFound -> "Foodstuff not found."
        
    let private serializeCreateError = function 
        | NameCannotBeEmpty -> ["Name cannot be empty."]
        | PersonCountMustBePositive -> ["Person count must be positive."]
        | InvalidImageUrl s -> [s]
        | InvalidUrl s -> [s]
        | AmountError e ->
            match e with
            | UnitCannotBeEmpty -> ["Unit cannot be empty."]
            | ValueCannotBeNegative -> ["Amount of ingredient must be positive."]
        | MustContaintAtLeastOneIngredient -> ["Must containt at least one ingredient."]
        | DescriptionIsProvidedButEmpty -> ["Description is provided but empty."]
        | DisplayLineOfIngredientIsProvidedButEmpty -> ["Description is provided but empty."]
        | CommentOfIngredientIsProvidedButEmpty -> ["Comment of ingredient is provided but empty."]
        | CookingTimeTextIsProvidedButEmpty -> ["Cooking time text is provided but empty."]
        | UnknownDifficulty -> ["Unknown difficulty."]
        | TagIsEmpty -> ["Tag is empty."]
        | InvalidRating -> ["Invalid rating."]
        | GramsMustBePositive -> ["Grams must be positive."]
        | PercentsMustBePositive -> ["Percents must be positive."]
        | CaloriesMustBePositive -> ["Calories must be positive."]
        | BusinessError e -> 
            match e with
            | Recipes.CreateError.Unauthorized -> ["Unauthorized."]
            | Recipes.CreateError.InvalidIngredients es -> List.map serializeCreateIngredientError es
        
    let private serializeCreate =
        Result.bimap (fun r -> { Recipe = serializeRecipe r }) (Seq.collect serializeCreateError)

    let create accessToken parameters = 
        RecipeParameters.parse parameters |> Validation.toResult |> ReaderT.id
        >>= createRecipe accessToken

    let createHandler<'a> =
        authorizedPostHandler create serializeCreate
