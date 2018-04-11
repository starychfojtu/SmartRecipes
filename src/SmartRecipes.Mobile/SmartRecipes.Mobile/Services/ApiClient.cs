using System.Net.Http;
using SmartRecipes.Mobile.ApiDto;
using System;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class ApiClient
    {
        private readonly HttpClient client;

        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        private string AuthenticationToken { get; set; }

        public async Task<SignInResponse> Post(SignInRequest request)
        {
            await SimulateRequest();

            //if (request.Email == "test@gmail.com" && request.Password == "1234")
            //{
            //AuthenticationToken = "fake";
            return new SignInResponse(true, AuthenticationToken);
            //}

            //return new SignInResponse(false, "");
        }

        public async Task<SignUpResponse> Post(SignUpRequest request)
        {
            await SimulateRequest();
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }

        public async Task<ShoppingListResponse> Post(AdjustItemInShoppingListRequest request)
        {
            await SimulateRequest();

            // TODO:
            //shoppingListItems = Ingredient.DecreaseAmount(item).Match(
            //    i => shoppingListItems.Replpace(item, i),
            //    () => shoppingListItems.Without(item).ToList()
            //);

            //TODO:
            //var increasedItem = Ingredient.IncreaseAmount(item);
            //var newItem = increasedItem.IfNone(() => throw new InvalidOperationException());
            //shoppingListItems = shoppingListItems.Replpace(item, newItem);

            //TODO:
            //var newItem = Ingredient.Create(foodstuff);
            //shoppingListItems.Add(newItem);

            return await GetShoppingList();
        }

        public async Task<ShoppingListResponse> GetShoppingList()
        {
            await SimulateRequest();
            var tomato = new ShoppingListResponse.Item.Foodstuff(
                Guid.NewGuid(),
                "Tomato",
                new Uri("https://vignette.wikia.nocookie.net/battlefordreamisland/images/0/0c/Tomato.PNG/revision/latest?cb=20170825141241"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            var onion = new ShoppingListResponse.Item.Foodstuff(
                Guid.NewGuid(),
                "Onion",
                new Uri("http://cdn.shopify.com/s/files/1/1537/5553/products/00613_15abd93a-e239-45df-acdb-8485b40d546a_grande.jpg?v=1486440965"),
                new Amount(1, AmountUnit.Piece),
                new Amount(1, AmountUnit.Piece)
            );
            var chickenBreast = new ShoppingListResponse.Item.Foodstuff(
                Guid.NewGuid(),
                "Chicken breast",
                new Uri("https://images-na.ssl-images-amazon.com/images/I/719JxkiwTVL._SL1500_.jpg"),
                new Amount(1000, AmountUnit.Gram),
                new Amount(100, AmountUnit.Gram)
            );

            var t = new ShoppingListResponse.Item(tomato, new Amount(3, AmountUnit.Piece));
            var o = new ShoppingListResponse.Item(onion, new Amount(2, AmountUnit.Piece));
            var b = new ShoppingListResponse.Item(chickenBreast, new Amount(600, AmountUnit.Gram));

            return new ShoppingListResponse(new[] { t, o, b });
        }

        private Task SimulateRequest()
        {
            return client.GetAsync("https://google.com");
        }
    }
}
