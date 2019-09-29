namespace SmartRecipes.UseCases

module Recipes =
    open System
    open SmartRecipes.IO
    open FSharpPlus.Data
    open Infrastructure
    open FSharpPlus
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.NaturalNumber
    open SmartRecipes.Domain.Recipe
                
    // Get all by account
    
    type GetMyRecipesError =
        | Unauthorized

    let getMyRecipes accessToken =
        Users.authorize Unauthorized accessToken
        >>= (Recipes.getByAccount >> IO.toSuccessEIO)
    
    // Get by ids
    
    type GetByIdsError = 
        | Unauthorized

    let getByIds accessToken ids = 
        Users.authorize Unauthorized accessToken
        >>= fun _ -> Recipes.getByIds ids |> IO.toSuccessEIO
        
    // Search
    
    type SearchError = 
        | Unauthorized

    let search accessToken query =
        Users.authorize Unauthorized accessToken
        >>= fun _ -> Recipes.search query |> IO.toSuccessEIO
        
    // Create
    
    type CreateIngredientError = 
        | DuplicateFoodstuffIngredient
        | FoodstuffNotFound
    
    type CreateError =
        | Unauthorized
        | InvalidIngredients of CreateIngredientError list
        
    type IngredientParameters = {
        FoodstuffId: Guid
        Amount: Amount option
        Comment: NonEmptyString option
        DisplayLine: NonEmptyString
    }
    
    let createIngredientParameters foodstuffId amount comment displayLine = {
        FoodstuffId = foodstuffId
        Amount = amount
        Comment = comment
        DisplayLine = displayLine
    }
    
    type RecipeParameters = {
        Name: NonEmptyString
        PersonCount: NaturalNumber
        ImageUrl: Uri option
        Url: Uri option
        Description: NonEmptyString option
        Ingredients: NonEmptyList<IngredientParameters>
        Difficulty: Difficulty option
        CookingTime: CookingTime option
        Tags: NonEmptyString seq
        Rating: Rating option
        NutritionPerServing: NutritionPerServing
    }
    
    let createParameters name personCount imageUrl url description ingredients difficulty cookingTime tags rating nutrition = {
        Name = name
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

    let private getFoodstuffs parameters =
        Seq.map (fun i -> i.FoodstuffId) parameters
        |> Foodstuffs.getByIds
        |> IO.toSuccessEIO

    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredient foodstuffMap parameters =
        Recipe.createIngredient
        <!> mkFoodstuffId parameters.FoodstuffId foodstuffMap
        <*> (Success parameters.Amount)
        <*> (Success parameters.DisplayLine)
        <*> (Success parameters.Comment)
    
    let private checkIngredientsNotDuplicate ingredients =
        let foodstuffIds = NonEmptyList.map (fun (i: Ingredient) -> i.FoodstuffId) ingredients
        if NonEmptyList.isDistinct foodstuffIds 
            then Success ingredients 
            else Failure [DuplicateFoodstuffIngredient]
            
    let private mkIngredients parameters foodstuffs =
        let foodstuffMap = Seq.map (fun (f: Foodstuff) -> (f.id.value, f)) foodstuffs |> Map.ofSeq
        NonEmptyList.map (mkIngredient foodstuffMap) parameters 
        |> Validation.traverseNonEmptyList
        |> Validation.bind checkIngredientsNotDuplicate
        |> Validation.mapFailure InvalidIngredients
        |> Validation.toResult
        |> IO.fromResult
            
    let private createIngredients parameters =
        getFoodstuffs parameters
        >>= (mkIngredients parameters)

    let private createRecipe parameters ingredients accountId = 
        Recipe.create
            parameters.Name
            accountId
            parameters.PersonCount
            ingredients
            parameters.Description
            parameters.CookingTime
            parameters.NutritionPerServing
            parameters.Difficulty
            (Seq.map RecipeTag parameters.Tags)
            parameters.ImageUrl
            parameters.Url
            parameters.Rating
        
    let create accessToken parameters = monad {
        let! accountId = Users.authorize Unauthorized accessToken
        let! ingredients = createIngredients parameters.Ingredients
        let recipe = createRecipe parameters ingredients accountId
        return! Recipes.add recipe |> IO.toSuccessEIO
    }
    
    // Update
    
    type UpdateError = 
        | RecipeNotFound
        | InvalidParameters of CreateError
    
    let update accessToken parameters =
        // TODO
        ()
    
    // Delete
    
    let delete accessToken parameters =
        // TODO
        ()