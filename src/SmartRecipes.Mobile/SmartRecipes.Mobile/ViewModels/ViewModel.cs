using System;
using System.ComponentModel;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static IAccount clientAccount;

        protected static IAccount CurrentAccount
        {
            get { return clientAccount ?? (clientAccount = new Account(Guid.Parse("13cb78ee-0aca-4287-9ecb-b87b4e83411b"), "someEmail@gmail.com")); }
            set { clientAccount = value; }
        }

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
