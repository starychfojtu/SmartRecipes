module DataAccess.Database
    open MongoDB.Driver
    open MongoDB.FSharp
    
    let mutable private database: IMongoDatabase = null
    
    let private initialize () =
        let prodConnectionString = "mongodb://prod:smartrecipes45@ds213053.mlab.com:13053/heroku_st1qt292"
        // let localConnectionString = "mongodb://localhost"
        let client = new MongoClient(prodConnectionString)
        // client.GetDatabase("SmartRecipes")
        client.GetDatabase("heroku_st1qt292")
        
    let getCollection<'a> () =
        let database = initialize ()
        database.GetCollection<'a> typeof<'a>.Name
        