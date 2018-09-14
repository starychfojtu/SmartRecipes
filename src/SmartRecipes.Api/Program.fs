module SmartRecipes.Api.App
    open System.Text
    open Api
    open Api
    open Api
    open System
    open System.IO
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Cors.Infrastructure
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open Giraffe
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.HttpOverrides
    
    let webApp =
        choose [
            GET >=> 
                choose [
                    route "/recipes" >=> Recipes.indexHandler
                ]  
            POST >=>
                choose [
                    route "/signUp" >=> Users.signUpHandler
                    route "/signIn" >=> Users.signInHandler
                    route "/foodstuffs" >=> Foodstuffs.createHandler
                    route "/recipes" >=> Recipes.createHandler
                    route "/shoppingList/addFoodstuffs" >=> ShoppingLists.addFoodstuffsHandler
                    route "/shoppingList/addRecipes" >=> ShoppingLists.addRecipesHandler
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
    
    let configureCors (builder : CorsPolicyBuilder) =
        builder
            .WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            |> ignore
    
    let configureApp (app : IApplicationBuilder) =
        let forwardedHeaderOptions = ForwardedHeadersOptions()
        forwardedHeaderOptions.ForwardedHeaders = (ForwardedHeaders.XForwardedFor ||| ForwardedHeaders.XForwardedProto) |> ignore
    
        let env = app.ApplicationServices.GetService<IHostingEnvironment>()
        (match env.IsDevelopment() with
        | true  -> app.UseDeveloperExceptionPage()
        | false -> app.UseGiraffeErrorHandler errorHandler)
            .UseForwardedHeaders(forwardedHeaderOptions)
            .UseCors(configureCors)
            .UseStaticFiles()
            .UseGiraffe(webApp)
    
    let configureServices (services : IServiceCollection) =
        services
            .AddCors()
            .AddGiraffe() 
            |> ignore
    
    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Error
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore
    
    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot = Path.Combine(contentRoot, "WebRoot")
        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRoot)
            .UseIISIntegration()
            .UseWebRoot(webRoot)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()
        0