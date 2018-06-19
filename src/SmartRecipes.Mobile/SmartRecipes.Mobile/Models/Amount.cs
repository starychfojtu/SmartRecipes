using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.Models
{
    public struct Amount : IAmount
    {
        public Amount(int count, AmountUnit unit)
        {
            Unit = unit;
            Count = count < 0 ? 0 : count;
        }

        public int Count { get; }

        public AmountUnit Unit { get; }

        public IAmount WithCount(int count)
        {
            return new Amount(count, Unit);
        }

        public override string ToString()
        {
            return $"{Count} {Unit.ToString()}";
        }

        // Combinators
        // TODO: make monoid

        public static IAmount Zero(AmountUnit unit)
        {
            return new Amount(0, unit);
        }

        public static bool IsLessThan(IAmount a1, IAmount a2)
        {
            return a1.Unit == a2.Unit && a1.Count < a2.Count;
        }

        public static bool Equals(IAmount a1, IAmount a2)
        {
            return a1.Unit == a2.Unit && a1.Count == a2.Count;
        }

        public static bool IsLessThanOrEquals(IAmount a1, IAmount a2)
        {
            return IsLessThan(a1, a2) || Equals(a1, a2);
        }

        public static Option<IAmount> Add(IAmount a1, IAmount a2)
        {
            return CountOperation((c1, c2) => c1 + c2, a1, a2);
        }

        public static Option<IAmount> Substract(IAmount a1, IAmount a2)
        {
            return CountOperation((c1, c2) => c1 - c2, a1, a2);
        }

        private static Option<IAmount> CountOperation(Func<int, int, int> op, IAmount first, IAmount second)
        {
            var validOperation = first.Unit == second.Unit;
            return validOperation
                ? Some((IAmount)new Amount(op(first.Count, second.Count), first.Unit))
                : None;
        }
    }
}
