module Infrastructure.NonEmptyList

    open FSharpPlus.Data
    open FSharpPlus.Operators
    
    type NonEmptyListError =
        | SequenceIsEmpty

    let mkNonEmptyList (seq: seq<'a>) =
        match Seq.toList seq with
        | [] -> Failure SequenceIsEmpty
        | x::xs -> Success (NonEmptyList.create x xs)
        
    let isDistinct nonEmptyList = 
        let list = NonEmptyList.toList nonEmptyList
        List.length list = (List.distinct list |> List.length)
        
      