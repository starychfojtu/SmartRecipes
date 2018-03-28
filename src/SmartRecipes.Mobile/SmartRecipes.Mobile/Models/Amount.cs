using System;
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class Amount
    {
        public Amount(int count, AmountUnit unit)
        {
            Unit = unit;
            Count = count;
        }

        public int Count { get; }

        public AmountUnit Unit { get; }

        public override string ToString()
        {
            return $"{Count} {Unit.ToString()}";
        }

        // Combinators

        public static Option<Amount> Add(Amount a1, Amount a2)
        {
            return CountOperation((c1, c2) => c1 + c2, a1, a2);
        }

        public static Option<Amount> Substract(Amount a1, Amount a2)
        {
            return CountOperation((c1, c2) => c1 - c2, a1, a2);
        }

        private static Option<Amount> CountOperation(Func<int, int, int> op, Amount first, Amount second)
        {
            var validOperation = first.Unit != second.Unit;
            return validOperation
                ? Option<Amount>.Some(new Amount(op(first.Count, second.Count), first.Unit))
                : Option<Amount>.None;
        }
    }
}
