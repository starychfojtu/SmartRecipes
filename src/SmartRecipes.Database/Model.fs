open System

[<CLIMutable>]
type Account = {
    id: Guid;
    email: string;
    password: string;
}

type MetricUnit = 
    | Liter = 0
    | Gram = 1
    | Piece = 2

[<CLIMutable>]
type Amount = {
    unit: MetricUnit;
    amount: float
}

[<CLIMutable>]
type Foodstuff = {
    id: Guid
    name: string;
    baseAmount: Amount
    amountStep: Amount
}

[<CLIMutable>]
type Recipe = {
    id: Guid;
    name: string;
    creatorId: Guid;
}

[<CLIMutable>]
type Ingredient = {
    id: Guid;
    recipeId: Guid;
    foodstuffId: Guid;
    amount: Amount;
}