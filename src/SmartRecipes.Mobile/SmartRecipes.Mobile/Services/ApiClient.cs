using System.Net.Http;
using SmartRecipes.Mobile.ApiDto;
using System;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Services
{
    // TODO: make static
    public class ApiClient
    {
        private readonly HttpClient client;

        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        private string AuthenticationToken { get; set; }

        public async Task<Option<SignInResponse>> Post(SignInRequest request)
        {
            return new SignInResponse(true, "");

            /* 
            if (request.Email == "test@gmail.com" && request.Password == "1234")
            {
                return new SignInResponse(true, AuthenticationToken);
            }

            return new SignInResponse(false, "");
            */
        }

        public async Task<Option<SignUpResponse>> Post(SignUpRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return None;
            }
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }

        public async Task<Option<ShoppingListResponse>> Post(ChangeFoodstuffAmountRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return None;
            }
            return await GetShoppingList();
        }

        public async Task<Option<ShoppingListResponse>> GetShoppingList()
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return None;
            }

            var tomato = new ShoppingListResponse.Item.Foodstuff(
                Guid.Parse("54ba369a-035f-4928-ac43-732ae234a5b8"),
                "Tomato",
                new Uri("https://vignette.wikia.nocookie.net/battlefordreamisland/images/0/0c/Tomato.PNG/revision/latest?cb=20170825141241"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            var onion = new ShoppingListResponse.Item.Foodstuff(
                Guid.Parse("581bc6d4-548a-4e33-a007-2980a7d1144b"),
                "Onion",
                new Uri("http://cdn.shopify.com/s/files/1/1537/5553/products/00613_15abd93a-e239-45df-acdb-8485b40d546a_grande.jpg?v=1486440965"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            var chickenBreast = new ShoppingListResponse.Item.Foodstuff(
                Guid.Parse("ae067aca-6430-402d-a7a9-54683efcac18"),
                "Chicken breast",
                new Uri("https://images-na.ssl-images-amazon.com/images/I/719JxkiwTVL._SL1500_.jpg"),
                new Amount(1000, AmountUnit.Gram),
                new Amount(100, AmountUnit.Gram)
            );

            var t = new ShoppingListResponse.Item(Guid.Parse("360a79f5-4dbb-441d-952f-acff818bfff5"), tomato, new Amount(3, AmountUnit.Piece));
            var o = new ShoppingListResponse.Item(Guid.Parse("a90f8456-d705-4c4c-aaf9-2bc4318326e7"), onion, new Amount(2, AmountUnit.Piece));
            var b = new ShoppingListResponse.Item(Guid.Parse("94c72dad-d77c-4012-b4f3-4730f591c02b"), chickenBreast, new Amount(600, AmountUnit.Gram));

            return new ShoppingListResponse(new[] { t, o, b });
        }

        public async Task<Option<MyRecipesResponse>> GetMyRecipes()
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return None;
            }

            var owner = new Account(Guid.Parse("13cb78ee-0aca-4287-9ecb-b87b4e83411b"), "someEmail@gmail.com");
            var imageUrl = "https://www.recipetineats.com/wp-content/uploads/2017/05/Lasagne-recipe-3-main-680x952.jpg";
            var recipes = new[]
            {
                new MyRecipesResponse.Recipe(Guid.Parse("a198fb84-42ca-41f8-bf23-2df76eb59b96"), "Lasagna", new Uri(imageUrl), owner.Id, 1, "Cook me"),
                new MyRecipesResponse.Recipe(Guid.Parse("110d81a1-a18b-43fb-9435-83ea8a1d4678"), "Lasagna 2", new Uri(imageUrl), owner.Id, 2, "Cook me twice")
            };
            return new MyRecipesResponse(recipes);
        }

        public async Task<Option<SearchFoodstuffResponse>> SearchFoodstuffs(SearchFoodstuffRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return None;
            }

            return new SearchFoodstuffResponse(new[]
            {
                new SearchFoodstuffResponse.Foodstuff(
                    Guid.Parse("cb3d0f54-c99d-43f1-ade4-e316b0e6543d"),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                new SearchFoodstuffResponse.Foodstuff(
                    Guid.Parse("e04ef558-1305-408e-9d26-1f04b7e3f785"),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            });
        }

        private Task SimulateRequest()
        {
            // TODO: Implement returning None when request fails
            return client.GetAsync("https://google.com");
        }
    }
}
