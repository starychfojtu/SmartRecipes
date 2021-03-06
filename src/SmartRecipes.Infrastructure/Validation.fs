module Infrastructure.Validation
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure
    open System

    let mapFailure mapper =
        bimap (fun e -> mapper e) (fun s -> s)
        
    let forceSucces = function 
        | Success s -> s
        | Failure f -> raise (InvalidOperationException())
        
    let traverseSeq seqOfValidation =
        Seq.traverse Validation.toResult seqOfValidation |> Validation.ofResult
        
    let traverseNonEmptyList (listOfValidation: NonEmptyList<'a>) =
        traverseSeq listOfValidation |> Validation.map (NonEmptyList.mkNonEmptyList >> forceSucces)
        
    let traverseOption validationOfOption =
        Option.traverse Validation.toResult validationOfOption |> Validation.ofResult
        
    let ofOption e = function
        | Some s -> Success s
        | None -> Failure e
        
    