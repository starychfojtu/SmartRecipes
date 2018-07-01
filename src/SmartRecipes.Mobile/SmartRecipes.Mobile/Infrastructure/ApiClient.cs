using System.Net.Http;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;

namespace SmartRecipes.Mobile.Infrastructure
{
    // TODO: make static
    public class ApiClient
    {
        private readonly HttpClient client;

        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Option<SignInResponse>> Post(SignInRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                // Do nothing for now
            }

            if (request.Email == "test@gmail.com" && request.Password == "1234")
            {
                return new SignInResponse(true, "");
            }

            return new SignInResponse(false, "");
        }

        public async Task<Option<SignUpResponse>> Post(SignUpRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return Prelude.None;
            }

            return Prelude.None;
        }

        public async Task<Option<ShoppingListResponse>> Post(ChangeFoodstuffAmountRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return Prelude.None;
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
                return Prelude.None;
            }

            return Prelude.None;
        }

        public async Task<Option<MyRecipesResponse>> GetMyRecipes()
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return Prelude.None;
            }

            return Prelude.None;
        }

        public async Task<Option<SearchFoodstuffResponse>> SearchFoodstuffs(SearchFoodstuffRequest request)
        {
            try
            {
                await SimulateRequest();
            }
            catch (HttpRequestException)
            {
                return Prelude.None;
            }

            return Prelude.None;
        }

        private Task SimulateRequest()
        {
            // API is not connected yet - turned off
            throw new HttpRequestException();
        }
    }
}
