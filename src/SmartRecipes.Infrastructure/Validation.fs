module Infrastructure.Validation
    open FSharpPlus.Data.Validation

    let mapFailure mapper =
        bimap (fun e -> mapper e) (fun s -> s)