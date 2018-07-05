[<RequireQualifiedAccess>]
module Business.Recipes
    open Models.Recipe
    open System

    let private getId = Guid.NewGuid();

    let create name creatorId = { 
        id = RecipeId getId;
        name = name;
        creatorId = creatorId
    }