using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ReadModels
{
    public class FoodstuffRepository : Repository
    {
        public FoodstuffRepository(ApiClient apiClient, Database database) : base(apiClient, database)
        {
        }

        public async Task<IEnumerable<IFoodstuff>> Search(string query)
        {
            // TODO: implement with retrieval action and API
            return new[]
            {
                Foodstuff.Create(
                    Guid.Parse("cb3d0f54-c99d-43f1-ade4-e316b0e6543d"),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                Foodstuff.Create(
                    Guid.Parse("e04ef558-1305-408e-9d26-1f04b7e3f785"),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            };
        }
    }
}
