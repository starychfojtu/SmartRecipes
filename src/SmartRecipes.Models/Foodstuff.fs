namespace SmartRecipes.Models

type MetricUnit = 
    | Liter 
    | Gram
    | Piece

type MetricUnitValue = {
    unit: MetricUnit;
    amount: int
}

type NutritionValues = {
    calories: int;
}

type Foodstuff = {
    name: string;
    nutritionValues: NutritionValues;
    baseAmount: MetricUnitValue
}