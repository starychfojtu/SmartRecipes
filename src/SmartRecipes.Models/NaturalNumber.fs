module Models.NaturalNumber
    open FSharpPlus.Data
    open System
    
    type NaturalNumber = NaturalNumber of uint16
    
    type NaturalNumberError =
        | NumberIsNotNatural
    
    let mkNaturalNumber n = 
        if n > 0
            then Success (NaturalNumber (Convert.ToUInt16 n))
            else Failure NumberIsNotNatural