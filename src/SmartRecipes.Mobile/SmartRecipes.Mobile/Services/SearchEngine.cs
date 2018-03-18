using System;
using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public class SearchEngine
    {
        private readonly ApiClient client;

        public SearchEngine(ApiClient client)
        {
            this.client = client;
        }

        public IEnumerable<Foodstuff> Search(string query)
        {
            // TODO: implement with API

            return new[]
            {
                new Foodstuff(
                    Guid.NewGuid(),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                new Foodstuff(
                    Guid.NewGuid(),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            };
        }
    }
}
