using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.Models
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
        // TODO: make monoid

        public static Amount Zero(AmountUnit unit)
        {
            return new Amount(0, unit);
        }

        public static bool IsLessThan(Amount a1, Amount a2)
        {
            return a1.Unit == a2.Unit && a1.Count < a2.Count;
        }

        public static bool Equals(Amount a1, Amount a2)
        {
            return a1.Unit == a2.Unit && a1.Count == a2.Count;
        }

        public static bool IsLessThanOrEquals(Amount a1, Amount a2)
        {
            return IsLessThan(a1, a2) || Equals(a1, a2);
        }

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
            var validOperation = first.Unit == second.Unit;
            return validOperation
                ? Some(new Amount(op(first.Count, second.Count), first.Unit))
                : None;
        }
    }
}
