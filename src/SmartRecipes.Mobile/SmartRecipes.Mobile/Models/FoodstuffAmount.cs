using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public abstract class FoodstuffAmount : IFoodstuffAmount
    {
        protected FoodstuffAmount(Guid id, Guid foodstuffId, IAmount amount)
        {
            Id = id;
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public FoodstuffAmount() { /* SQLite */ }

        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid FoodstuffId { get; set; }

        public int _Count { get; set; }
        public AmountUnit _Unit { get; set; }
        [Ignore]
        public IAmount Amount
        {
            get { return new Amount(_Count, _Unit); }
            set
            {
                _Count = value.Count;
                _Unit = value.Unit;
            }
        }
    }
}
