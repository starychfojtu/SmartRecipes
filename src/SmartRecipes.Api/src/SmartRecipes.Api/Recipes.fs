module Api.Recipes
    open Api
    open Domain
    open Generic
    open System.Net.Http
    open NonEmptyString
    open DataAccess
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open UseCases
    open Context
    open DataAccess.Foodstuffs
    open Domain
    open Domain.Foodstuff
    open Domain.Recipe
    open FSharpPlus.Data
    open Infrastructure
    open UseCases.Recipes
    open Infrastructure.Validation
    open NaturalNumber
    open Uri
    open NonNegativeFloat
    open FSharpPlus
            
    // Get by account        
    
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let private getGetByAccountDao (): GetByAccountDao = {
        tokens = (Tokens.getDao ())
        recipes = (Recipes.getDao ())
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler (getGetByAccountDao ()) next ctx (fun token p ->
            Recipes.getAllbyAccount token p.accountId)
            
    // Create
    
    [<CLIMutable>]
    type IngredientParameter = {
        foodstuffId: Guid
        amount: float
    }

    [<CLIMutable>]
    type CreateParameters = {
        name: string
        personCount: int
        imageUrl: string
        description: string
        ingredients: seq<IngredientParameter>
    }
    
    type CreateError =
        | NameCannotBeEmpty
        | PersonCountMustBePositive
        | InvalidImageUrl of string
        | AmountOfIngredientMustBePositive
        | MustContaintAtLeastOneIngredient
        | FoodstuffNotFound
        | DescriptionIsProvidedButEmpty
        | BusinessError of Recipes.CreateError
        
    let private createParameters name personCount imageUrl description ingredients: Recipes.CreateParameters = {
        name = name
        personCount = personCount
        imageUrl = imageUrl
        description = description
        ingredients = ingredients
    }
    
    let private getFoodstuff parameters = 
        Reader(fun (dao: FoodstuffDao) -> Seq.map (fun i -> i.foodstuffId) parameters |> dao.getByIds )
        
    let private createIngredientParameter foodstuffId amount: Recipe.IngredientParameter = {
        foodstuffId = foodstuffId
        amount = amount
    }
    
    let mkFoodstuffId guid (foodstuffMap: Map<_, Foodstuff> ) = 
        match Map.tryFind guid foodstuffMap with  
        | Some f -> Success f.id
        | None -> Failure [FoodstuffNotFound]
        
    let private mkIngredientParameters foodstuffMap parameters =
        createIngredientParameter
        <!> mkFoodstuffId parameters.foodstuffId foodstuffMap
        <*> (mkNonNegativeFloat parameters.amount |> mapFailure (fun _ -> [AmountOfIngredientMustBePositive]))
        
    let private mkAllIngredientParameters parameters foodstuffMap =
        Seq.map (mkIngredientParameters foodstuffMap) parameters

    let private parseIngredientParameters parameters =
        let toMap = Seq.map (fun (f: Foodstuff) -> (f.id.value, f)) >> Map.ofSeq
        // check not empty
        getFoodstuff parameters
        |> Reader.map toMap
        |> Reader.map (mkAllIngredientParameters parameters)
        
    let private mkDescription d =
        if isNull d then Success None else mkNonEmptyString d |> mapFailure (fun _ -> [DescriptionIsProvidedButEmpty]) |> map Some 
        
    let private parseParameters (parameters: CreateParameters): Recipes.CreateParameters =
        createParameters
        <!> (mkNonEmptyString parameters.name |> mapFailure (fun _ -> [NameCannotBeEmpty]))
        <*> (mkNaturalNumber parameters.personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]))
        <*> (mkUri parameters.imageUrl |> mapFailure (fun m -> [InvalidImageUrl(m)]))
        <*> mkDescription parameters.description
        <*> parseIngredientParameters parameters.ingredients
    
//    let private createIngredient recipeId parameter =
//        mkIngredient recipeId parameter.foodstuff.id 
//        <!> mkAmount parameter.amount parameter.foodstuff.baseAmount.unit
//        
//    let private createIngredients parameters recipeInfo =
//        parameters
//        |> Seq.traverse (fun i -> 
//            createIngredient recipeInfo.id i
//            |> mapFailure (fun _ -> [AmountOfFoodstuffMustBePositive]) 
//            |> Validation.toResult)
//        |> Validation.ofResult
//    
//        let ingredients = 
//            Validation.bind (createIngredients ingredientParameters) recipeInfo
//            |> Validation.bind (fun s -> mkNonEmptyList s |> mapFailure (fun _ -> [MustContaintAtLeastOneIngredient]))
//        
//    let private crateParameters parameters token foodstuffs =
//        Seq.exactJoin foodstuffs (fun f -> f.id.value) parameters (fun i -> i.foodstuffId) (fun (f, p) -> mkParameter f p.amount) 
//        |> Option.map (fun f -> (token, f))
//        |> toResult [FoodstuffNotFound]
//    
//    let private mapParameters parameters token =
//        Seq.map (fun i -> i.foodstuffId) parameters
//        |> Foodstuffs.getByIds
//        |> Reader.map (crateParameters parameters token)
//        
//    let createHandler (next : HttpFunc) (ctx : HttpContext) =
//        authorizedPostHandler next ctx (fun parameters accessToken ->
//            Recipes.create accessToken (toInfoParemeters parameters) parameters.ingredients)
