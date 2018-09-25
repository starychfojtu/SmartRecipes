module DataAccess.Database
    open MongoDB.Driver
    open MongoDB.FSharp
    
    let mutable database: IMongoDatabase = null
        
    let getCollection<'a> () =
        database.GetCollection<'a> typeof<'a>.Name
        