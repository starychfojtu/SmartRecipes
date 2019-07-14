namespace SmartRecipes.Api

module App =
    open Giraffe
    open System
    open System.IO
    open Microsoft.AspNetCore
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open MongoDB.Driver
    open SmartRecipes
    open SmartRecipes.DataAccess
    
    let webApp =
        choose [
            GET >=> 
                choose [
                    route "/recipes/my" >=> Recipes.GetMyRecipes.handler
                    route "/recipes" >=> Recipes.GetByIds.handler
                    route "/recipes/search" >=> Recipes.Search.handler
                    route "/shoppingList" >=> ShoppingLists.getHandler
                    route "/foodstuffs" >=> Foodstuffs.GetByIds.handler
                    route "/foodstuffs/search" >=> Foodstuffs.Search.handler
                ]  
            POST >=>
                choose [
                    route "/signUp" >=> Users.SignUp.handler
                    route "/signIn" >=> Users.SignIn.handler
                    route "/foodstuffs" >=> Foodstuffs.Create.handler
                    route "/recipes" >=> Recipes.createHandler
                    route "/shoppingList/addFoodstuffs" >=> ShoppingLists.addFoodstuffsHandler
                    route "/shoppingList/changeAmount" >=> ShoppingLists.changeAmountHandler
                    route "/shoppingList/addRecipes" >=> ShoppingLists.addRecipesHandler
                    route "/shoppingList/changePersonCount" >=> ShoppingLists.changePersonCountHandler
                    route "/shoppingList/removeFoodstuff" >=> ShoppingLists.removeFoodstuffHandler
                    route "/shoppingList/removeRecipe" >=> ShoppingLists.removeRecipeHandler
                ]
            setStatusCode 404 >=> text "Not Found"
        ]
    
    // ---------------------------------
    // Error handler
    // ---------------------------------
    
    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
    
    // ---------------------------------
    // Config and Main
    // ---------------------------------
    
    let configureDatabase isProd =
        let connStr = 
            if isProd
            then
                let vars = System.Environment.GetEnvironmentVariables ();
                string vars.["MONGODB_URI"]
            else 
                "mongodb://localhost"
        let database = MongoClient(connStr).GetDatabase("SmartRecipes")
        Database.database <- database // TODO: I am ugly, refactor me
    
    let configureApp (app : IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IHostingEnvironment>()
        configureDatabase (env.IsProduction()) |> ignore
        (match env.IsDevelopment() with
        | true  -> app.UseDeveloperExceptionPage()
        | false -> app.UseGiraffeErrorHandler errorHandler)
            .UseStaticFiles()
            .UseGiraffe(webApp)
    
    let configureServices (services : IServiceCollection) =
        services
            .AddGiraffe()
            |> ignore
    
    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Error
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore
        
    let getWebHost () = 
        WebHost
            .CreateDefaultBuilder()
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            
    [<EntryPoint>]
    let main _ =
        let webHost = getWebHost ()
        let vars = System.Environment.GetEnvironmentVariables ()
        let webHostWithSentry = 
            if vars.Contains("SENTRY")
            then webHost.UseSentry(string vars.["SENTRY"])
            else webHost
        
        webHostWithSentry
            .Build()
            .Run()
        0