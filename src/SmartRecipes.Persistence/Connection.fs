module Connection

let connString  = "Server=localhost;Database=smartrecipes;User=root;Password=";

type sql = SqlDataProvider<
                dbVendor,
                connString,
                ResolutionPath = resPath,
                IndividualsAmount = indivAmount,
                UseOptionTypes = useOptTypes,
                Owner = "smartrecipes"
            >
let ctx = sql.GetDataContext()

let employees = 
    ctx.smartrecipes.employees 
    |> Seq.map (fun e -> e.ColumnValues |> Seq.toList)
    |> Seq.toList