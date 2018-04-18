﻿using System.Net.Http;
using SmartRecipes.Mobile.ApiDto;
using System;
using System.Threading.Tasks;
using LanguageExt;

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

        public async Task<Option<SignInResponse>> Post(SignInRequest request)
        {
            await SimulateRequest();

            //if (request.Email == "test@gmail.com" && request.Password == "1234")
            //{
            //AuthenticationToken = "fake";
            return new SignInResponse(true, AuthenticationToken);
            //}

            //return new SignInResponse(false, "");
        }

        public async Task<Option<SignUpResponse>> Post(SignUpRequest request)
        {
            await SimulateRequest();
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }

        public async Task<Option<ShoppingListResponse>> Post(AdjustIngredientRequest request)
        {
            await SimulateRequest();
            return await GetShoppingList();
        }

        public async Task<Option<ShoppingListResponse>> GetShoppingList()
        {
            await SimulateRequest();
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
