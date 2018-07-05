using System;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Infrastructure
{
    public static class FakeData
    {
        public static IEnumerable<IFoodstuff> FakeFoodstuffs()
        {
            yield return Foodstuff.Create(
                Guid.Parse("54ba369a-035f-4928-ac43-732ae234a5b8"),
                "Tomato",
                new Uri("https://vignette.wikia.nocookie.net/battlefordreamisland/images/0/0c/Tomato.PNG/revision/latest?cb=20170825141241"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            yield return Foodstuff.Create(
                Guid.Parse("581bc6d4-548a-4e33-a007-2980a7d1144b"),
                "Onion",
                new Uri("http://cdn.shopify.com/s/files/1/1537/5553/products/00613_15abd93a-e239-45df-acdb-8485b40d546a_grande.jpg?v=1486440965"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            yield return Foodstuff.Create(
                Guid.Parse("ae067aca-6430-402d-a7a9-54683efcac18"),
                "Chicken breast",
                new Uri("https://images-na.ssl-images-amazon.com/images/I/719JxkiwTVL._SL1500_.jpg"),
                new Amount(1000, AmountUnit.Gram),
                new Amount(100, AmountUnit.Gram)
            );
            yield return Foodstuff.Create(
                Guid.Parse("cb3d0f54-c99d-43f1-ade4-e316b0e6543d"),
                "Carrot",
                new Uri("https://www.marshalls-seeds.co.uk/images/products/product_9338.jpg"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            yield return Foodstuff.Create(
                Guid.Parse("e04ef558-1305-408e-9d26-1f04b7e3f785"),
                "Bacon",
                new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                new Amount(100, AmountUnit.Gram),
                new Amount(50, AmountUnit.Gram)
            );
        }

        public static IAccount FakeAccount()
        {
            return new Account(Guid.Parse("13cb78ee-0aca-4287-9ecb-b87b4e83411b"), "test@gmail.com");
        }

        public static IEnumerable<Recipe> FakeRecipes()
        {
            var imageUrl = "https://d2mkh7ukbp9xav.cloudfront.net/recipeimage/mik71rvx-06e07-496278-cfcd2-6tl9kcup/c15867d1-e1dd-452e-8820-980858391160/main/the-perfect-lasagne.jpg";
            yield return Recipe.Create(Guid.Parse("a198fb84-42ca-41f8-bf23-2df76eb59b96"), FakeAccount().Id, "Lasagna", new Uri(imageUrl), 1, "Cook me");
        }
    }
}
