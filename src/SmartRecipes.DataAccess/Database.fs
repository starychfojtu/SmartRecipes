namespace SmartRecipes.DataAccess

module Database =
    open MongoDB.Driver
    
    let mutable database: IMongoDatabase = null
        
    let getCollection<'a> () =
        database.GetCollection<'a> typeof<'a>.Name
        