namespace SmartRecipes.Api

open System.Threading.Tasks
open FSharp.Json
open Infrastructure
open SmartRecipes.Domain
open Errors
open Npgsql
open Npgsql.FSharp

module Parse =
    let option v =
        Option.map v >> Validation.traverseOption

    let seqOf v =
        Seq.map v >> Validation.traverseSeq

    let nonEmptyString error =
        NonEmptyString.create >> Validation.ofOption error

    let nonEmptyStringOption error =
        nonEmptyString error |> option

    let uriOption error =
        option (Uri.create >> Validation.mapFailure error)

    let naturalNumber error =
        NaturalNumber.create >> Validation.ofOption error

    let nonNegativeFloat error =
        NonNegativeFloat.create >> Validation.ofOption error

module Environment =

    open System
    open SmartRecipes.IO
    open SmartRecipes.IO.Users
    open SmartRecipes.IO.Tokens
    open SmartRecipes.IO.Foodstuffs
    open SmartRecipes.IO.Recipes
    open SmartRecipes.IO.ShoppingLists
    open SmartRecipes.DataAccess
    open SmartRecipes.UseCases.DateTimeProvider

    type ProductionEnvironment(conn, recipeStatistics, foodstuffVectors) =

        member this.Conn = conn
        member this.RecipeStatistics: SmartRecipes.Recommendations.TfIdf.DataSetStatistics = recipeStatistics
        member this.FoodstuffVectors = foodstuffVectors

        interface IUserDao with
            member e.getById id = Users.Postgres.getById e.Conn id
            member e.getByEmail email = Users.Postgres.getByEmail e.Conn email
            member e.add user = Users.Postgres.add e.Conn user

        interface ITokensDao with
            member e.get v = Tokens.Postgres.get e.Conn v
            member e.add user = Tokens.Postgres.add e.Conn user

        interface IFoodstuffDao with
            member e.getByIds ids = Foodstuffs.Postgres.getByIds e.Conn ids |> List.toSeq
            member e.search q = Foodstuffs.Postgres.search e.Conn q |> List.toSeq
            member e.add f = failwith "Not implemented"
            member e.getVectors () = e.FoodstuffVectors

        interface IRecipesDao with
            member e.getByIds ids =
                let idList = Seq.toList ids
                e.RecipeStatistics.Recipes |> Seq.filter (fun r -> List.contains r.Id.value idList)
            member e.getByAccount acc = failwith "Not implemented"
            member e.search q =
                e.RecipeStatistics.Recipes |> Seq.filter (fun r -> r.Name.Value.Contains(q.Value))
            member e.add r = failwith "Not implemented"
            member e.getRecommendationStatistics () = e.RecipeStatistics

        interface IShoppingsListsDao with
            member e.getByAccount acc = ShoppingLists.Postgres.getByAccount e.Conn acc
            member e.update l = ShoppingLists.Postgres.update e.Conn l
            member e.add l = ShoppingLists.Postgres.add e.Conn l

        interface IDateTimeProvider with
            member p.nowUtc () = DateTime.UtcNow

    let getConnection () =
        let vars = System.Environment.GetEnvironmentVariables ()
        let postgresPassword = string vars.["POSTGRES_PASSWORD"]
        Sql.host "postgresql-9457-0.cloudclusters.net"
        |> Sql.port 9472
        |> Sql.username "admin"
        |> Sql.password postgresPassword
        |> Sql.database "smartrecipes"
        |> Sql.sslMode SslMode.Disable
        |> Sql.connectFromConfig

    let recipeStatistics =
        let connection = getConnection ()
        let recipes = Recipes.Postgres.getAllRecipes connection
        SmartRecipes.Recommendations.TfIdf.computeStatistics recipes

    let foodstuffVectors =
        SmartRecipes.Recommendations.FoodToVector.Data.loadFoodstuffVectors "vectors.txt"
        |> Seq.map (fun kvp -> (SmartRecipes.Domain.Foodstuff.FoodstuffId kvp.Key, kvp.Value))
        |> Map.ofSeq

    let getEnvironment () =
        ProductionEnvironment(getConnection (), recipeStatistics, foodstuffVectors)

module Generic =
    open Giraffe
    open Microsoft.AspNetCore.Http
    open FSharpPlus.Data
    open FSharp.Control.Tasks
    open Environment

    let private deserialize<'a> json =
        try
            Ok <| Json.deserialize<'a> json
        with
            | ex -> Error ex.Message

    let private serializeToJson obj =
        Json.serializeEx (JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)) obj

    let private setStatusCode (ctx: HttpContext) code =
        ctx.SetStatusCode code

    let private bindQueryString<'a> (ctx: HttpContext) =
        try
            ctx.BindQueryString<'a>()
        with ex -> failwith ex.Message

    let private bindModelAsync<'a> (ctx: HttpContext): Task<Result<'a, string>> = task {
        let! body = ctx.ReadBodyFromRequestAsync ()
        return deserialize body
    }

    let private getHeader (ctx: HttpContext) name =
        ctx.GetRequestHeader(name)

    let private getResult env next ctx handler (serialize: 'c -> Result<'d, Errors.Error>) parameters =
        let result = handler parameters |> ReaderT.execute env |> serialize
        let response =
            match result with
            | Ok s -> setStatusCode ctx 200 |> (fun _ -> text <| serializeToJson s)
            | Error e -> setStatusCode ctx 400 |> (fun _ -> text <| serializeToJson e)
        response next ctx

    let getHandler handler serialize next ctx =
        bindQueryString<'parameters> ctx |> getResult (getEnvironment ()) next ctx handler serialize

    let postHandler handler serialize next ctx =
        task {
            let! parameters = bindModelAsync ctx
            return!
                match parameters with
                | Ok p -> getResult (getEnvironment ()) next ctx handler serialize p
                | Error e -> getResult () next ctx (fun _ -> error e |> Error |> ReaderT.id) id ()
        }

    let authorizedGetHandler handler serialize next ctx =
        let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
        getHandler (handler accessToken) serialize next ctx

    let authorizedPostHandler handler serialize next ctx =
        let accessToken = match getHeader ctx "authorization" with Ok t -> t | Error _ -> ""
        postHandler (handler accessToken) serialize next ctx