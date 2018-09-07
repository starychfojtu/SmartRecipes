module Api.Recipes
    open Api
    open Generic
    open System.Net.Http
    open Business
    open DataAccess
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open UseCases
    open Context
    open FSharpPlus.Data
    open UseCases.Recipes
            
    // Get by account        
    
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let private getGetAllByAccountDao () = {
        tokens = (Tokens.getDao ())
        recipes = (Recipes.getDao ())
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler (getGetAllByAccountDao ()) next ctx (fun token p ->
            Recipes.getAllbyAccount token p.accountId)
            
    // Create
    
    [<CLIMutable>]
    type IngredientParameter = {
        foodstuff: Guid
        amount: float
    }
    
    [<CLIMutable>]
    type CreateParameters = {
        name: string
        personCount: int
        description: string
        imageUrl: string
        ingredients: seq<IngredientParameter>
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | AmountOfIngredientMustBePositive
        | MustContaintAtLeastOneIngredient
        | BusinessError of CreateError
        
    let private createIngredient recipeId parameter =
        mkIngredient recipeId parameter.foodstuff.id 
        <!> mkAmount parameter.amount parameter.foodstuff.baseAmount.unit
        
    let private createIngredients parameters recipeInfo =
        parameters
        |> Seq.traverse (fun i -> 
            createIngredient recipeInfo.id i
            |> mapFailure (fun _ -> [AmountOfFoodstuffMustBePositive]) 
            |> Validation.toResult)
        |> Validation.ofResult
        
    let private createRecipeInfo accountId parameters =
        mkRecipeInfo
        <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
        <*> Success accountId
        <*> (mkNaturalNumber parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> Success parameters.description
    
    let create accountId infoParameters ingredientParameters =
        let recipeInfo = 
            createRecipeInfo accountId infoParameters
        
        let ingredients = 
            Validation.bind (createIngredients ingredientParameters) recipeInfo
            |> Validation.bind (fun s -> mkNonEmptyList s |> mapFailure (fun _ -> [MustContaintAtLeastOneIngredient]))
        
        mkRecipe 
        <!> recipeInfo 
        <*> ingredients
        |> Validation.toResult
        
    let private crateParameters parameters token foodstuffs =
        Seq.exactJoin foodstuffs (fun f -> f.id.value) parameters (fun i -> i.foodstuffId) (fun (f, p) -> mkParameter f p.amount) 
        |> Option.map (fun f -> (token, f))
        |> toResult [FoodstuffNotFound]
    
    let private mapParameters parameters token =
        Seq.map (fun i -> i.foodstuffId) parameters
        |> Foodstuffs.getByIds
        |> Reader.map (crateParameters parameters token)
        
    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler next ctx (fun parameters accessToken ->
            Recipes.create accessToken (toInfoParemeters parameters) parameters.ingredients)