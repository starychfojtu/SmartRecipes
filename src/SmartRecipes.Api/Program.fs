module SmartRecipes.Api.App
    open System.Text
    open Giraffe
    open Api
    open System
    open System.IO
    open Microsoft.AspNetCore
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Cors.Infrastructure
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.HttpOverrides
    open MongoDB.Driver
    
    let webApp =
        choose [
            GET >=> 
                choose [
                    route "/recipes" >=> Recipes.getMyRecipesHandler
                    route "/shoppingList" >=> ShoppingLists.getHandler
                    route "/foodstuffs" >=> Foodstuffs.getByIdshandler
                    route "/foodstuffs/search" >=> Foodstuffs.searchHandler
                ]  
            POST >=>
                choose [
                    route "/signUp" >=> Users.signUpHandler
                    route "/signIn" >=> Users.signInHandler
                    route "/foodstuffs" >=> Foodstuffs.createHandler
                    route "/recipes" >=> Recipes.createHandler
                    route "/shoppingList/addFoodstuffs" >=> ShoppingLists.addFoodstuffsHandler
                    route "/shoppingList/changeAmount" >=> ShoppingLists.changeAmountHandler
                    route "/shoppingList/addRecipes" >=> ShoppingLists.addRecipesHandler
                    route "/shoppingList/changePersonCount" >=> ShoppingLists.changePersonCountHandler
                    route "/shoppingList/cook" >=> ShoppingLists.cookHandler
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
                let username = string(vars.["DATABASE_USERNAME"]);
                let password = string(vars.["DATABASE_PASSWORD"]);
                sprintf "mongodb://%s:%s@ds213053.mlab.com:13053/heroku_st1qt292" username password
            else 
                "mongodb://localhost"
        let databaseName = if isProd then "heroku_st1qt292" else "SmartRecipes"
        let database = MongoClient(connStr).GetDatabase(databaseName)
        DataAccess.Database.database <- database // TODO: I am ugly, refactor me
    
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
    
    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        WebHost.CreateDefaultBuilder()
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()
        0