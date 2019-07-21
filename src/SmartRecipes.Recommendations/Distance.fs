namespace SmartRecipes.Recommendations

open FSharpPlus.Data
open FSharpPlus.Operators
open FSharpPlus.Lens
open Input
open SmartRecipes.Domain.Recipe

module Distance =
    
    let private levenshtein array1 array2 =
        let m, n = List.length array1, List.length array2
        let table: int[,] = Array2D.zeroCreate (m + 1) (n + 1)
        let substitutionCost = 2
        for i in 0..m do
            for j in 0..n do
                match i, j with
                | i, 0 -> table.[i, j] <- i
                | 0, j -> table.[i, j] <- j
                | _, _ ->
                    let delete = table.[i - 1, j] + 1
                    let insert = table.[i, j - 1] + 1
                    let substitute =
                        let substitutionCost = if array1.[i - 1] = array2.[j - 1] then 0 else substitutionCost
                        table.[i - 1, j - 1] + substitutionCost
                    table.[i, j] <- List.min [delete; insert; substitute]
        table.[m, n]
        
    let evaluate input recipe =
        let ingredientIds = map (view _foodstuffId) recipe.Ingredients |> NonEmptyList.toList
        levenshtein ingredientIds input.FoodstuffIds