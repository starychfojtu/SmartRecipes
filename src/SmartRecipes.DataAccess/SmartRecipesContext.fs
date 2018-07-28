module DataAccess.Context
    open Microsoft.EntityFrameworkCore
    open FSharp.Core
    open Model
    open System

    type Context =
        inherit DbContext
        
        new() = { inherit DbContext() }
        new(options: DbContextOptions<Context>) = { inherit DbContext(options) }
    
        [<DefaultValue>]
        val mutable accounts:DbSet<DbAccount>
        member c.Accounts
            with get() = c.accounts 
            and set v = c.accounts <- v
            
        [<DefaultValue>]
        val mutable accessTokens:DbSet<DbAccessToken>
        member c.AccessTokens
            with get() = c.accessTokens 
            and set v = c.accessTokens <- v
    
        [<DefaultValue>]
        val mutable foodstuffs:DbSet<DbFoodstuff>
        member c.Foodstuffs
            with get() = c.foodstuffs
            and set v = c.foodstuffs <- v
    
        [<DefaultValue>]
        val mutable ingredients:DbSet<DbIngredient>
        member c.Ingredients 
            with get() = c.ingredients 
            and set v = c.ingredients <- v
    
        [<DefaultValue>]
        val mutable recipes:DbSet<DbRecipe>
        member c.Recipes 
            with get() = c.recipes 
            and set v = c.recipes <- v
    
        override this.OnModelCreating mb =
            mb.Entity<DbAccount>().HasKey(fun a -> a.id :> obj) |> ignore
            
            mb.Entity<DbAccessToken>().HasKey(fun t -> t.value :> obj) |> ignore
            mb.Entity<DbAccessToken>().HasOne<DbAccount>().WithMany().HasForeignKey(fun t -> t.accountId :> obj) |> ignore
    
            mb.Entity<DbFoodstuff>().HasKey(fun f -> f.id :> obj) |> ignore
            mb.Entity<DbFoodstuff>().OwnsOne(fun f -> f.amountStep) |> ignore
            mb.Entity<DbFoodstuff>().OwnsOne(fun f -> f.baseAmount) |> ignore
    
            mb.Entity<DbRecipe>().HasKey(fun r -> r.id :> obj) |> ignore
            mb.Entity<DbRecipe>().HasOne<DbAccount>().WithMany().HasForeignKey(fun r -> r.creatorId :> obj) |> ignore
            
            mb.Entity<DbIngredient>().HasKey(fun i -> i.id :> obj) |> ignore
            mb.Entity<DbIngredient>().HasOne<DbRecipe>().WithMany().HasForeignKey(fun r -> r.recipeId :> obj) |> ignore
            mb.Entity<DbIngredient>().HasOne<DbFoodstuff>().WithMany().HasForeignKey(fun r -> r.foodstuffId :> obj) |> ignore
            mb.Entity<DbIngredient>().OwnsOne(fun i -> i.amount) |> ignore

    let createDbContext () = 
        let optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder.UseInMemoryDatabase("SmartRecipes") |> ignore
        new Context(optionsBuilder.Options)