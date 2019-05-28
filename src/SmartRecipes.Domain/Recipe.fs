namespace SmartRecipes.Domain

module Recipe =
    open Account
    open System
    open FSharpPlus.Data
    open Foodstuff
    open NaturalNumber
    open NonEmptyString
    
    type Ingredient = {
        FoodstuffId: FoodstuffId
        Amount: Amount option // Sometimes it is not specified how much of it should be used, eg. 'Olive oil'.
        DisplayLine: NonEmptyString option // Original unstructured input.
        Comment: NonEmptyString option // Additional comments about the ingredient, e.g. fresh
    }
    
    let createIngredient foodstuffId amount displayLine comment = {
        FoodstuffId = foodstuffId
        Amount = amount
        DisplayLine = displayLine
        Comment = comment
    }
    
    type RecipeId = RecipeId of Guid
        with member i.value = match i with RecipeId v -> v
        
    type Difficulty =
        | Easy
        | Normal
        | Hard
        
    // For now this is only text, but in future is should be processed via NLP to get the real time and thereby extended.
    type CookingTime = {
        Text: NonEmptyString
    }
    
    module CookingTime =
        let create text = {
            Text = text
        }
    
    type RecipeTag =
        RecipeTag of NonEmptyString
        with member t.Value = match t with RecipeTag v -> v
    
    type Rating =
        private Rating of NaturalNumber
        with member r.Value = match r with Rating v -> v
        
    module Rating =
        let create v =
            NaturalNumber.create v
            |> Option.filter (fun n -> n.Value >= 1us && n.Value <= 10us)
            |> Option.map Rating
    
    type NutritionInfo = {
        Grams: NaturalNumber
        Percents: NaturalNumber option
    }
    
    module NutritionInfo =
        let create grams percents = {
            Grams = grams
            Percents = percents
        }
    
    type NutritionPerServing = {
        Calories: NaturalNumber option
        Fat: NutritionInfo option
        SaturatedFat: NutritionInfo option
        Sugars: NutritionInfo option
        Protein: NutritionInfo option
        Carbs: NutritionInfo option
    }
    
    module NutritionPerServing =
        let create calories fat saturatedFat sugars protein carbs = {
            Calories = calories
            Fat = fat
            SaturatedFat = saturatedFat
            Sugars = sugars
            Protein = protein
            Carbs = carbs
        }

    type Recipe = {
        Id: RecipeId
        Name: NonEmptyString
        CreatorId: AccountId
        PersonCount: NaturalNumber
        ImageUrl: Uri option
        Url: Uri option
        Description: NonEmptyString option
        Ingredients: Ingredient NonEmptyList
        Difficulty: Difficulty option
        CookingTime: CookingTime option
        Tags: RecipeTag seq
        Rating: Rating option
        NutritionPerServing: NutritionPerServing
    }
    
    let create name creatorId personCount ingredients description cookingTime nutrition difficulty tags imageUrl url rating  = {
        Id = RecipeId(Guid.NewGuid ())
        Name = name
        CreatorId = creatorId
        PersonCount = personCount
        ImageUrl = imageUrl
        Url = url
        Description = description
        Ingredients = ingredients
        Difficulty = difficulty
        CookingTime = cookingTime
        Tags = tags
        Rating = rating
        NutritionPerServing = nutrition
    }
        
        