module Infrastructure.Validation
    open FSharpPlus.Data
    open FSharpPlus.Data
    open FSharpPlus.Data
    open FSharpPlus.Data.Validation
    open Infrastructure
    open System

    let mapFailure mapper =
        bimap (fun e -> mapper e) (fun s -> s)
        
    let forceSucces = function 
        | Success s -> s
        | Failure f -> raise (InvalidOperationException())
        
    let traverse seqOfValidation =
        Seq.traverse Validation.toResult seqOfValidation |> Validation.ofResult
        
    let traverseNonEmptyList (listOfValidation: NonEmptyList<'a> ) =
        traverse listOfValidation |> Validation.map (NonEmptyList.mkNonEmptyList >> forceSucces)
        
    