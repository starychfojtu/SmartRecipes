module Infrastructure.Seq

    let join first keySelector second fkSelector project =
        let keyValuePairs = Seq.map (fun i -> (keySelector i, i)) first
        let keyMap = Map.ofSeq keyValuePairs
        Seq.map (fun i -> (Map.find (fkSelector i) keyMap, i) |> project) second
        
    let exactJoin first keySelector second fkSelector project =
        if Seq.length first = Seq.length second 
        then join first keySelector second fkSelector project |> Some
        else None