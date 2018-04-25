using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.ViewModels
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

    public static class ViewModelExtensions
    {
        public static void Bind<TViewModel, TProperty>(this TViewModel viewModel, BindableObject obj, BindableProperty property, Expression<Func<TViewModel, TProperty>> propertyAccessor)
            where TViewModel : ViewModel
        {
            obj.SetBinding(property, propertyAccessor.ToPropertyPathName());
        }
    }
}
