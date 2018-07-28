module Infrastructure.NonEmptyList
    open FSharpPlus.Data
    
    type NonEmptyListError =
        | SequenceIsEmpty
    
    // TODO: refactor
    let mkNonEmptyList (seq: seq<'a>) =
        match Seq.toList seq with
        | [] -> Failure SequenceIsEmpty
        | x::xs -> Success (NonEmptyList.create x xs)