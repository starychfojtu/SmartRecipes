using System.ComponentModel;

namespace SmartRecipes.Mobile
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected readonly Store store;

        public ViewModel(Store store)
        {
            this.store = store;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
