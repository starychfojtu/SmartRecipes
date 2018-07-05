module Database.Context
    open Microsoft.EntityFrameworkCore
    open FSharp.Core
    open Database.Model
    open System

    type Context =
        inherit DbContext
        
        new() = { inherit DbContext() }
        new(options: DbContextOptions<Context>) = { inherit DbContext(options) }
    
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
    
            mb.Entity<Foodstuff>().HasKey(fun f -> f.id :> obj) |> ignore
            mb.Entity<Foodstuff>().OwnsOne(fun f -> f.amountStep) |> ignore
            mb.Entity<Foodstuff>().OwnsOne(fun f -> f.baseAmount) |> ignore
    
            mb.Entity<Recipe>().HasKey(fun r -> r.id :> obj) |> ignore
            mb.Entity<Recipe>().HasOne<Account>().WithMany().HasForeignKey(fun r -> r.creatorId :> obj) |> ignore
            
            mb.Entity<Ingredient>().HasKey(fun i -> i.id :> obj) |> ignore
            mb.Entity<Ingredient>().HasOne<Recipe>().WithMany().HasForeignKey(fun r -> r.recipeId :> obj) |> ignore
            mb.Entity<Ingredient>().HasOne<Foodstuff>().WithMany().HasForeignKey(fun r -> r.foodstuffId :> obj) |> ignore
            mb.Entity<Ingredient>().OwnsOne(fun i -> i.amount) |> ignore

    let createContext = 
        let optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder.UseInMemoryDatabase("SmartRecipes") |> ignore
        new Context(optionsBuilder.Options)