module HttpHandlers

open Giraffe
open SmartRecipes.Models

    module Recipes =
        let index _ =
            let recipe = { name = "Lasagne"; ingredients = [] }
            json recipe