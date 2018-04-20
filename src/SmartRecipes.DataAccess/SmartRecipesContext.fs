namespace SmartRecipes.DataAccess

open Microsoft.EntityFrameworkCore
open FSharp.Core
open SmartRecipes.Models

type SmartRecipesContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<SmartRecipesContext>) = { inherit DbContext(options) }

    [<DefaultValue>]
    val mutable accounts:DbSet<Account>
    member c.Accounts
        with get() = c.accounts 
        and set v = c.accounts <- v

    [<DefaultValue>]
    val mutable foodstuff:DbSet<Foodstuff>
    member c.Foodstuff 
        with get() = c.foodstuff 
        and set v = c.foodstuff <- v

    [<DefaultValue>]
    val mutable recipeSteps:DbSet<RecipeStep>
    member c.RecipeSteps
        with get() = c.recipeSteps 
        and set v = c.recipeSteps <- v

    [<DefaultValue>]
    val mutable ingredients:DbSet<Ingredient>
    member c.Ingredients 
        with get() = c.ingredients 
        and set v = c.ingredients <- v

    [<DefaultValue>]
    val mutable recipes:DbSet<Recipe>
    member c.Recipes 
        with get() = c.recipes 
        and set v = c.recipes <- v

    override this.OnModelCreating mb =
        mb.Entity<Account>().HasKey(fun a -> a.id :> obj) |> ignore
        mb.Entity<Account>().OwnsOne(fun a -> a.signInInfo) |> ignore

        mb.Entity<Foodstuff>().HasKey(fun a -> a.id :> obj) |> ignore
        mb.Entity<Foodstuff>().OwnsOne(fun f -> f.nutritionValues) |> ignore
        mb.Entity<Foodstuff>().OwnsOne(fun f -> f.baseAmount).OwnsOne(fun a -> a.unit) |> ignore

        mb.Entity<RecipeStep>().HasKey(fun a -> a.id :> obj) |> ignore
        mb.Entity<RecipeStep>().HasOne(fun s -> s.recipe) |> ignore

        mb.Entity<Ingredient>().HasKey(fun a -> a.id :> obj) |> ignore
        mb.Entity<Ingredient>().HasOne(fun i -> i.recipe) |> ignore
        mb.Entity<Ingredient>().HasOne(fun i -> i.food) |> ignore

        mb.Entity<Recipe>().HasKey(fun a -> a.id :> obj) |> ignore
        mb.Entity<Recipe>().HasOne(fun r -> r.creator) |> ignore

 module SmartRecipesContext =

    let private connectionString = "Server=DESKTOP-I9VPKJO\SQLEXPRESS;Database=SmartRecipes;Trusted_Connection=True;"
    
    let create = 
        let optionsBuilder = new DbContextOptionsBuilder<SmartRecipesContext>();
        optionsBuilder.UseSqlServer(connectionString) |> ignore
        new SmartRecipesContext(optionsBuilder.Options)