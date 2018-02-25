namespace SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =
    open System
    open SmartRecipes.Models

    // Query

    type Query = {
        withId: Guid -> Recipe option;
        byAccount: Account -> Recipes;
    }

    let withId id (context: SmartRecipesContext) =
        context.Recipes |> Seq.find (fun r -> r.id = id)


    // Maybe will need query <@ @> fo EF
    // TODO make generic
    let private efQuery (context :SmartRecipesContext) = {
        withId = (fun id ->
            context.Recipes |> Seq.tryFind (fun r -> r.id = id)
        );
        byAccount = (fun account -> 
            context.Recipes |> Seq.filter (fun r -> r.creatorId = account.id)
        );
    }

    // Command

    type Command = {
        create: Recipe -> bool;
        update: Recipe -> bool;
        delete: Recipe -> bool;
    }

    // TODO make this generic
    let private efCommand (context :SmartRecipesContext) = {
        create = (fun recipe -> 
            context.Add(recipe) |> ignore
            context.SaveChanges() |> ignore
            true
        );
        update = (fun recipe ->
            context.SaveChanges() |> ignore
            true
        );
        delete = (fun recipe ->
            context.Remove(recipe) |> ignore
            context.SaveChanges() |> ignore
            true
        );
    }

    // API

    let query context = efQuery context
    let command context = efCommand context