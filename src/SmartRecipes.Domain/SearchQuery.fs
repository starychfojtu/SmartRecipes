namespace SmartRecipes.Domain

open System.Text.RegularExpressions
open NonEmptyString

module SearchQuery =
    type SearchQuery =
        private SearchQuery of NonEmptyString
        with member q.Value = match q with (SearchQuery v) -> v.Value
        
    let create (nonEmptyString: NonEmptyString) =
        Regex.Escape nonEmptyString.Value
        |> NonEmptyString.create
        |> Option.get
        |> SearchQuery