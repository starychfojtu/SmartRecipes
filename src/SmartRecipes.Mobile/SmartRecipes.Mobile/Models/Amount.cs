using System;

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

        public Amount Add(Amount amount)
        {
            return CountOperation((c1, c2) => c1 + c2, this, amount);
        }

        public Amount Substract(Amount amount)
        {
            return CountOperation((c1, c2) => c1 - c2, this, amount);
        }

        private Amount CountOperation(Func<int, int, int> op, Amount first, Amount second)
        {
            if (first.Unit != second.Unit)
            {
                throw new InvalidOperationException("Cannot add different amounts");
            }

            return new Amount(op(first.Count, second.Count), first.Unit);
        }

        public override string ToString()
        {
            return $"{Count} {Unit.ToString()}";
        }
    }
}
