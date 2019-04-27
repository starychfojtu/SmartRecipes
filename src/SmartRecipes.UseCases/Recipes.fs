namespace SmartRecipes.UseCases

module Recipes =
    open System
    open SmartRecipes.DataAccess.Foodstuffs
    open FSharpPlus.Data
    open Infrastructure
    open FSharpPlus
    open Infrastructure.Reader
    open SmartRecipes.DataAccess.Recipes
    open SmartRecipes.DataAccess.Tokens
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.NaturalNumber
    open SmartRecipes.Domain.NonNegativeFloat
    open SmartRecipes.Domain.Recipe
    open Environment
                
    // Get all by account
    
    type GetMyRecipesError =
        | Unauthorized
        
    let private getRecipes accountId = 
        Reader(fun env -> env.IO.Recipes.getByAccount accountId |> Ok)
        
    let getMyRecipes accessToken =
        Users.authorize Unauthorized accessToken
        >>=! getRecipes
        
    // Create
    
    type CreateIngredientError = 
        | DuplicateFoodstuffIngredient
        | FoodstuffNotFound
    
    type CreateError =
        | Unauthorized
        | InvalidIngredients of CreateIngredientError list
        
    type IngredientParameters = {
        foodstuffId: Guid
        amount: NonNegativeFloat
    }
    
    type RecipeParameters = {
        name: NonEmptyString
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<IngredientParameters>
    }

    let private getFoodstuff parameters =
        Reader(fun env -> Seq.map (fun i -> i.foodstuffId) parameters |> env.IO.Foodstuffs.getByIds )

    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredient foodstuffMap parameters =
        Recipe.createIngredient
        <!> mkFoodstuffId parameters.foodstuffId foodstuffMap
        <*> (Success parameters.amount)
    
    let private checkIngredientsNotDuplicate ingredients =
        let foodstuffIds = NonEmptyList.map (fun (i: Ingredient) -> i.foodstuffId) ingredients
        if NonEmptyList.isDistinct foodstuffIds 
            then Success ingredients 
            else Failure [DuplicateFoodstuffIngredient]
            
    let private mkIngredients parameters foodstuffMap =
        NonEmptyList.map (mkIngredient foodstuffMap) parameters 
        |> Validation.traverseNonEmptyList
        |> Validation.bind checkIngredientsNotDuplicate
        |> Validation.mapFailure InvalidIngredients
        |> Validation.toResult
            
    let private createIngredients parameters =
        let toMap = Seq.map (fun (f: Foodstuff) -> (f.id.value, f)) >> Map.ofSeq
        getFoodstuff parameters
        |> Reader.map toMap
        |> Reader.map (mkIngredients parameters)

    let private createRecipe parameters ingredients accountId = 
        Recipe.createRecipe parameters.name accountId parameters.personCount parameters.imageUrl parameters.description ingredients
        |> Ok 
        |> Reader.id

    let private addToDatabase recipe = 
        Reader(fun env -> env.IO.Recipes.add recipe |> Ok)
        
    let private createNewRecipe accessToken parameters =
        Users.authorize Unauthorized accessToken
        >>=! (fun a -> createIngredients parameters.ingredients |> Reader.map (Result.map (fun i -> (i, a)))) 
        >>=! (fun (ingredients, accountId) -> createRecipe parameters ingredients accountId)

    let create accessToken parameters =
        createNewRecipe accessToken parameters
        >>=! addToDatabase
        
    // Update
    
    type UpdateError = 
        | RecipeNotFound
        | InvalidParameters of CreateError
    
    let update accessToken parameters =
        // verify recipe exists
        createNewRecipe accessToken parameters
        // replace in database
    
    // Delete
    
    let delete accessToken parameters =
        // check if exists
        // remove from all shopping lists
        // delete
        ()