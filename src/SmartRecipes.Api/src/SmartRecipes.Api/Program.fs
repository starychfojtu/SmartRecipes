module SmartRecipes.Api.App
    open Api
    open System
    open System.IO
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Cors.Infrastructure
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open Giraffe
    open Microsoft.AspNetCore.Authentication
    open Microsoft.AspNetCore.Authentication.JwtBearer
    open Microsoft.AspNetCore.Http
    open Microsoft.IdentityModel.Tokens
    open Models.User
    
    type Claim = { accountId: AccountId }
    type SimpleClaim = { Type: string; Value: string }
    
    let authorize =
        requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)
    
    let showClaims =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let simpleClaims = 
                ctx.User.Claims
                |> Seq.map (fun (i : Security.Claims.Claim) -> {Type = i.Type; Value = i.Value})
            json simpleClaims next ctx
    
    let webApp =
        choose [
            GET >=> 
                choose [
                    route "/claims" >=> authorize >=> showClaims
                ]  
            POST >=>
                choose [
                    route "/signUp" >=> Users.signUpHandler
                    route "/signIn" >=> Users.signInHandler
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
        builder.WithOrigins("http://localhost:8080")
               .AllowAnyMethod()
               .AllowAnyHeader()
               |> ignore
    
    let configureApp (app : IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IHostingEnvironment>()
        (match env.IsDevelopment() with
        | true  -> app.UseDeveloperExceptionPage()
        | false -> app.UseGiraffeErrorHandler errorHandler)
            .UseCors(configureCors)
            .UseStaticFiles()
            .UseAuthentication()
            .UseGiraffe(webApp)
    
    let authenticationOptions (o : AuthenticationOptions) =
        o.DefaultAuthenticateScheme <- JwtBearerDefaults.AuthenticationScheme
        o.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme
    
    let jwtBearerOptions (cfg : JwtBearerOptions) =
        cfg.SaveToken <- true
        cfg.IncludeErrorDetails <- true
        cfg.Authority <- "https://accounts.google.com"
        cfg.Audience <- "your-oauth-2.0-client-id.apps.googleusercontent.com"
        cfg.TokenValidationParameters <- TokenValidationParameters (
            ValidIssuer = "accounts.google.com"
        )
    
    let configureServices (services : IServiceCollection) =
        services
            .AddCors()
            .AddGiraffe()
            .AddAuthentication(authenticationOptions)
            .AddJwtBearer(Action<JwtBearerOptions> jwtBearerOptions) |> ignore
    
    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Error
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore
    
    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot     = Path.Combine(contentRoot, "WebRoot")
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