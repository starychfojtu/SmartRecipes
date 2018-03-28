using System;

namespace SmartRecipes.Mobile
{
    /// <summary>
    /// The only place where mutable transactions, handles state.
    /// </summary>
    public partial class Store
    {
        private readonly ApiClient apiClient;

        public Store(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public event EventHandler StateChanged;
    }
}
