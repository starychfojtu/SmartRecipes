using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
    public class ViewModel : BindableObject
    {
        protected readonly Store store;

        public ViewModel(Store store)
        {
            this.store = store;
            store.StateChanged += (s, e) => OnPropertyChanged();
        }
    }
}
