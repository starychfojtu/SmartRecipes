module DataAccess.Database
    open MongoDB.Driver
    open MongoDB.FSharp
    
    let mutable private database: IMongoDatabase = null
    
    let private initialize () =
        Serializers.Register()
        let connectionString = "mongodb://localhost"
        let client = new MongoClient(connectionString)
        database = client.GetDatabase("SmartRecipes")
        
    let getCollection<'a> () =
        if isNull database then initialize () |> ignore
        database.GetCollection<'a> typeof<'a>.Name
        