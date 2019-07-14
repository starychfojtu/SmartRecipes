namespace SmartRecipes.Api

module Errors =

    type ParameterError = {
        message: string
        parameter: string
    } 

    type Error = {
        message: string
        parameterErrors: ParameterError list
    }
    
    let error s = {
        message = s
        parameterErrors = List.empty
    }
    
    let invalidParameters errors = {
        message = "Invalid parameters."
        parameterErrors = Seq.toList errors
    }
    
    let parameterError s parameter = {
        message = s
        parameter = parameter
    }
