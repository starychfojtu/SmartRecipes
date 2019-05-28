namespace SmartRecipes.Api
open FSharpPlus.Data
open SmartRecipes.Domain.NaturalNumber
open SmartRecipes.Domain.NonEmptyString

module Dto =
    open System
    open System.Globalization
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Account
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.Recipe
    open SmartRecipes.Domain.ShoppingList
    open SmartRecipes.Domain.Token
    
    let private serializeDateTime (d: DateTime) =
        d.ToString("s", CultureInfo.InvariantCulture)
    
    type AccountDto = {
        id: string
        email: string
    }
    
    let serializeAccount (account: Account) = {
        id = account.id.value.ToString ()
        email = account.credentials.email.Address
    }
    
    type AccessTokenDto = {
        value: string
        accountId: string
        expirationUtc: string
    }
    
    let serializeAccessToken (accessToken: AccessToken) = {
        value = accessToken.value.value
        accountId = accessToken.accountId.value.ToString ()
        expirationUtc = accessToken.expirationUtc |> serializeDateTime
    }
    
    type AmountDto = {
        value: float
        unit: string
    }
    
    let serializeAmount (amount: Amount) = {
        value = NonNegativeFloat.value amount.value
        unit = amount.unit.Value.Value
    }
    
    type FoodstuffDto = {
        id: string
        name: string
        baseAmount: AmountDto
        amountStep: float
    }
    
    let serializeFoodstuff (foodstuff: Foodstuff) = {
        id = foodstuff.id.value.ToString ()
        name = foodstuff.name.Value
        baseAmount = serializeAmount foodstuff.baseAmount
        amountStep = NonNegativeFloat.value foodstuff.amountStep
    }
    
    type IngredientDto = {
        foodstuffId: string
        amount: AmountDto option
    }
    
    let serializeIngredient (ingredient: Ingredient) = {
        foodstuffId = ingredient.FoodstuffId.value.ToString ()
        amount = Option.map serializeAmount ingredient.Amount
    }
    
    type CookingTimeDto = {
        Text: string
    }
    
    let serializeCookingTime (time: CookingTime) = {
        Text = time.Text.Value
    }
    
    type NutritionInfoDto = {
        Grams: int
        Percents: int option
    }
    
    let serializeNutritionInfo (info: NutritionInfo) = {
        Grams = int info.Grams.Value
        Percents = Option.map (fun (p: NaturalNumber) -> int p.Value) info.Percents
    }
    
    type NutritionPerServingDto = {
        Calories: int option
        Fat: NutritionInfoDto option
        SaturatedFat: NutritionInfoDto option
        Sugars: NutritionInfoDto option
        Protein: NutritionInfoDto option
        Carbs: NutritionInfoDto option
    }
    
    let private serializeNutritionPerServing (nutrition: NutritionPerServing): NutritionPerServingDto = {
        Calories = Option.map (fun (n: NaturalNumber) -> int n.Value) nutrition.Calories
        Fat = Option.map serializeNutritionInfo nutrition.Fat
        SaturatedFat = Option.map serializeNutritionInfo nutrition.SaturatedFat
        Sugars = Option.map serializeNutritionInfo nutrition.Sugars
        Protein = Option.map serializeNutritionInfo nutrition.Protein
        Carbs = Option.map serializeNutritionInfo nutrition.Carbs
    }
    
    type RecipeDto = {
        Id: string
        Name: string
        CreatorId: string
        PersonCount: int
        ImageUrl: string
        Url: string
        Description: string
        Ingredients: IngredientDto seq
        Difficulty: string
        Rating: int option
        Tags: string seq
        CookingTime: CookingTimeDto option
        NutritionPerServing: NutritionPerServingDto
    }
    
    let private serializeDifficulty = function
        | Difficulty.Easy -> "easy"
        | Difficulty.Normal -> "normal"
        | Difficulty.Hard -> "hard"
    
    let serializeRecipe (recipe: Recipe): RecipeDto = {
        Id = recipe.Id.value.ToString ()
        Name = recipe.Name.Value
        CreatorId = recipe.CreatorId.value.ToString ()
        PersonCount = Convert.ToInt32 recipe.PersonCount
        ImageUrl = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.ImageUrl |> Option.toObj
        Url = Option.map (fun (u: Uri) -> u.AbsoluteUri) recipe.Url |> Option.toObj
        Description = Option.map (fun (s: NonEmptyString) -> s.Value) recipe.Description |> Option.toObj
        Ingredients = NonEmptyList.map serializeIngredient recipe.Ingredients |> NonEmptyList.toSeq
        Difficulty = Option.map serializeDifficulty recipe.Difficulty |> Option.toObj
        CookingTime = Option.map serializeCookingTime recipe.CookingTime
        Tags = Seq.map (fun (t: RecipeTag) -> t.Value.Value) recipe.Tags
        Rating = Option.map (fun (r: Rating) -> int r.Value.Value) recipe.Rating
        NutritionPerServing = serializeNutritionPerServing recipe.NutritionPerServing
    }
    
    type ListItemDto  = {
        foodstuffId: string
        amount: float
    }
     
    let serializeListItem (i: ListItem) = {
        foodstuffId = i.foodstuffId.value.ToString ()
        amount = NonNegativeFloat.value i.amount
    }
    
    type RecipeListItemDto  = {
        recipeId: string
        personCount: int
    }
     
    let serializeRecipeListItem (i: RecipeListItem) = {
        recipeId = i.recipeId.value.ToString ()
        personCount = int i.personCount.Value
    }
    
    type ShoppingListDto = {
        id: string
        ownerId: string
        items: seq<ListItemDto>
        recipes: seq<RecipeListItemDto>
    }
    
    let serializeShoppingList (s: ShoppingList): ShoppingListDto = {
        id = s.id.value.ToString ()
        ownerId = s.accountId.value.ToString ()
        items = Seq.map serializeListItem (Map.toSeq s.items |> Seq.map (fun (_, i) -> i))
        recipes = Seq.map serializeRecipeListItem (Map.toSeq s.recipes |> Seq.map (fun (_, i) -> i))
    }