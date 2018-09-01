module UseCases.Foodstuffs
    open Infrastructure
    open Models
    open UseCases
    open Models.Foodstuff
    open Models.NonEmptyString
    open Infrastructure.Validation
    open Infrastructure.Reader

    type CreateParameters = {
        name: NonEmptyString
        baseAmount: Amount
        amountStep: Amount
    }
    
    type CreateError = 
        | NotAuthorized
        | FoodstuffAlreadyExists
        
    let createFoodstuff parameters =
        createFoodstuff parameters.name parameters.baseAmount parameters.amountStep
        |> Ok
        |> Reader.id

    let ensureDoesntAlreadyExists foodstuff =
        Ok foodstuff |> Reader.id // TODO: implement
        
    let addToDatabase foodstuff = 
        Ok foodstuff |> Reader.id // TODO: implement

    let create accessToken parameters = 
        Users.authorize NotAuthorized accessToken
        >>=! (fun _ -> createFoodstuff parameters)
        >>=! ensureDoesntAlreadyExists
        >>=! addToDatabase