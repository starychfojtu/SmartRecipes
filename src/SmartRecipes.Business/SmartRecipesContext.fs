namespace SmartRecipes.DataAccess

open Microsoft.EntityFrameworkCore
open FSharp.Core
open SmartRecipes.Models

type SmartRecipesContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<SmartRecipesContext>) = { inherit DbContext(options) }

    [<DefaultValue>]
    val mutable recipes:DbSet<Recipe>
    member c.Recipes 
        with get() = c.recipes 
        and set v = c.recipes <- v
