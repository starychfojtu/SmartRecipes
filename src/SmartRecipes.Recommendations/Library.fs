module SmartRecipes.Recommendations.Library

open System
open System.Diagnostics

let mapSecond f (fst, snd) = (fst, f snd)
let first (fst, _) = fst
let second (_, snd) = snd

let magnitude vector =
    vector
    |> Map.toList
    |> List.map (second)
    |> List.map (fun v -> Math.Pow(v, 2.0))
    |> List.sum
    |> Math.Sqrt

let dotProduct v1 v2 =
    let getKeys map = Map.toSeq map |> Seq.map first
    Seq.append (getKeys v1) (getKeys v2)
    |> Seq.distinct
    |> Seq.map (fun k ->
        match (Map.tryFind k v1), (Map.tryFind k v2) with
        | Some a, Some b -> Some <| a * b
        | _ -> None)
    |> Seq.collect (function Some a -> [a] | None -> [])
    |> Seq.sum

let cosineSimilarity v1 v2 =
    (dotProduct v1 v2) / ((magnitude v1) * (magnitude v2))

let euclideanDistance v1 v2 =
    let getKeys map = Map.toSeq map |> Seq.map first
    Seq.append (getKeys v1) (getKeys v2)
    |> Seq.distinct
    |> Seq.map (fun k ->
        match (Map.tryFind k v1), (Map.tryFind k v2) with
        | Some a, Some b -> Math.Pow(a - b, 2.0)
        | Some a, None -> Math.Pow(a, 2.0)
        | None, Some b -> Math.Pow(b, 2.0)
        | None, None -> 0.0)
    |> Seq.sum
    |> Math.Sqrt

type Vector = Map<Guid, float>

let profilePerformance f =
    let sw = Stopwatch.StartNew ()
    let result = f ()
    sw.Stop()
    (result, sw.ElapsedMilliseconds)