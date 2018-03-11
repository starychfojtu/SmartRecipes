using System.Net.Http;
using SmartRecipes.Mobile.ApiDto;
using System;

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

        public SignInResponse PostSignIn(SignInRequest request)
        {
            //if (request.Email == "test@gmail.com" && request.Password == "1234")
            //{
            //AuthenticationToken = "fake";
            return new SignInResponse(true, AuthenticationToken);
            //}

            //return new SignInResponse(false, "");
        }

        public SignUpResponse PostSignUp(SignUpRequest request)
        {
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }

        public ShoppingListResponse GetShoppingList()
        {
            var tomato = new ShoppingListResponse.Item.Foodstuff(Guid.NewGuid(), "Tomato", "https://vignette.wikia.nocookie.net/battlefordreamisland/images/0/0c/Tomato.PNG/revision/latest?cb=20170825141241");
            var onion = new ShoppingListResponse.Item.Foodstuff(Guid.NewGuid(), "Onion", "http://cdn.shopify.com/s/files/1/1537/5553/products/00613_15abd93a-e239-45df-acdb-8485b40d546a_grande.jpg?v=1486440965");
            var chickenBreast = new ShoppingListResponse.Item.Foodstuff(Guid.NewGuid(), "Chicken breast", "https://images-na.ssl-images-amazon.com/images/I/719JxkiwTVL._SL1500_.jpg");

            var t = new ShoppingListResponse.Item(tomato, 3);
            var o = new ShoppingListResponse.Item(onion, 2);
            var b = new ShoppingListResponse.Item(chickenBreast, 1);

            return new ShoppingListResponse(new[] { t, o, b });
        }
    }
}
