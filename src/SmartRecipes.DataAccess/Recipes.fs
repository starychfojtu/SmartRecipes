namespace SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =
    open System
    open SmartRecipes.Models

    // Query

    type Query = {
        withId: Guid -> Recipe option;
        byAccount: Account -> Recipe seq;
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
            context.Recipes |> Seq.filter (fun r -> r.creator = account)
        );
    }

    // Command

    type Command = {
        create: Recipe -> unit;
        update: Recipe -> unit;
        delete: Recipe -> unit;
    }

    // TODO make this generic
    let private efCommand (context :SmartRecipesContext) = {
        create = (fun recipe -> 
            context.Add(recipe) |> ignore
            context.SaveChanges() |> ignore
        );
        update = (fun recipe ->
            context.SaveChanges() |> ignore
        );
        delete = (fun recipe ->
            context.Remove(recipe) |> ignore
            context.SaveChanges() |> ignore
        );
    }

    // API

    let query = efQuery SmartRecipesContext.create
    let command = efCommand SmartRecipesContext.create