namespace SmartRecipes.Api

module Errors =

    type ParameterError = {
        message: string
        parameter: string
    } 

    type Error = {
        message: string
        parameterErrors: seq<ParameterError>
    }
    
    let error s = {
        message = s
        parameterErrors = Seq.empty
    }
    
    let invalidParameters errors = {
        message = "Invalid parameters."
        parameterErrors = errors
    }
    
    let parameterError s parameter = {
        message = s
        parameter = parameter
    }
