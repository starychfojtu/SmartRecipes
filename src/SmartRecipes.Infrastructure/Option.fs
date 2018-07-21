module Infrastructure.Option

    let toResult error option = 
        match option with
        | Some s -> Ok s 
        | None -> Error error