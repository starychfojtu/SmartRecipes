namespace SmartRecipes.Domain

open NonNegativeFloat

module Recipe =
    open Account
    open System
    open FSharpPlus.Data
    open FSharpPlus
    open Foodstuff
    open NaturalNumber
    open NonEmptyString
    
    type Ingredient = {
        FoodstuffId: FoodstuffId
        // Sometimes it is not specified how much of it should be used, eg. 'Olive oil'.
        Amount: Amount option
        // Original unstructured input.
        DisplayLine: NonEmptyString
        // Additional comments about the ingredient, e.g. fresh
        Comment: NonEmptyString option 
    }
       
    let inline _foodstuffId f ingredient = map (fun v -> { ingredient with FoodstuffId = v }) (f ingredient.FoodstuffId) 
    
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
        Grams: NonNegativeFloat
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
        Salt: NutritionInfo option
        Protein: NutritionInfo option
        Carbs: NutritionInfo option
        Fibre: NutritionInfo option
    }
    
    module NutritionPerServing =
        let create calories fat saturatedFat sugars salt protein carbs fibre = {
            Calories = calories
            Fat = fat
            SaturatedFat = saturatedFat
            Sugars = sugars
            Salt = salt
            Protein = protein
            Carbs = carbs
            Fibre = fibre
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
    
    let inline _ingredients f recipe = map (fun v -> { recipe with Ingredients = v }) (f recipe.Ingredients) 
    
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
        
        