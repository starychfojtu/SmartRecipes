using System;
namespace SmartRecipes.Mobile.ApiDto
{
    public class SearchFoodstuffRequest
    {
        public SearchFoodstuffRequest(string query)
        {
            Query = query;
        }

        public string Query { get; }
    }
}
