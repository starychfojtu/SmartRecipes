using System.ComponentModel;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class ViewModel : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task InitializeAsync()
        {
            return Task.FromResult(false);
        }
    }
}
