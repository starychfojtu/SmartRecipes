module DataAccess.Database
    open MongoDB.Driver
    open MongoDB.FSharp
    
    let mutable private database: IMongoDatabase = null
    
    let private initialize () =
        let connectionString = "mongodb://localhost"
        let client = new MongoClient(connectionString)
        client.GetDatabase("SmartRecipes")
        
    let getCollection<'a> () =
        let database = initialize ()
        database.GetCollection<'a> typeof<'a>.Name
        