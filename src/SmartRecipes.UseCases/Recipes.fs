namespace SmartRecipes.UseCases

module Recipes =
    open System
    open SmartRecipes.DataAccess.Foodstuffs
    open FSharpPlus.Data
    open Infrastructure
    open FSharpPlus
    open SmartRecipes.DataAccess.Recipes
    open SmartRecipes.Domain
    open SmartRecipes.Domain.Foodstuff
    open SmartRecipes.Domain.NonEmptyString
    open SmartRecipes.Domain.NaturalNumber
    open SmartRecipes.Domain.NonNegativeFloat
    open SmartRecipes.Domain.Recipe
    open Environment
    open FSharpPlus
                
    // Get all by account
    
    type GetMyRecipesError =
        | Unauthorized
        
    let private getRecipes accountId = 
        ReaderT(fun env -> env.IO.Recipes.getByAccount accountId |> Ok)
        
    let getMyRecipes accessToken =
        Users.authorize Unauthorized accessToken
        >>= getRecipes
        
    // Create
    
    type CreateIngredientError = 
        | DuplicateFoodstuffIngredient
        | FoodstuffNotFound
    
    type CreateError =
        | Unauthorized
        | InvalidIngredients of CreateIngredientError list
        
    type IngredientParameters = {
        foodstuffId: Guid
        amount: Amount option
    }
    
    type RecipeParameters = {
        name: NonEmptyString
        personCount: NaturalNumber
        imageUrl: Uri
        description: NonEmptyString option
        ingredients: NonEmptyList<IngredientParameters>
    }

    let private getFoodstuff parameters =
        Reader(fun env -> Seq.map (fun i -> i.foodstuffId) parameters |> env.IO.Foodstuffs.getByIds)

    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredient foodstuffMap parameters =
        Recipe.createIngredient
        <!> mkFoodstuffId parameters.foodstuffId foodstuffMap
        <*> (Success parameters.amount)
    
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
        |> ReaderT.id
            
    let private createIngredients parameters =
        getFoodstuff parameters
        |> ReaderT.hoist
        |> ReaderT.bind (mkIngredients parameters)

    let private createRecipe parameters ingredients accountId = 
        Recipe.createRecipe parameters.name accountId parameters.personCount parameters.imageUrl parameters.description ingredients

    let private addToDatabase recipe = 
        ReaderT(fun env -> env.IO.Recipes.add recipe |> Ok)
        
    let create accessToken parameters = monad {
        let! accountId = Users.authorize Unauthorized accessToken
        let! ingredients = createIngredients parameters.ingredients
        let recipe = createRecipe parameters ingredients accountId
        return! addToDatabase recipe
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
        // check if exists
        // remove from all shopping lists
        // delete
        ()