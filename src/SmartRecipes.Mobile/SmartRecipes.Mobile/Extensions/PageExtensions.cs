using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Extensions
{
    public static class PageExtensions
    {
        public static async Task LoaderAction(ActivityIndicator indicator, Func<Task> a)
        {
            indicator.IsRunning = true;
            await a();
            indicator.IsRunning = false;
        }
    }
}
