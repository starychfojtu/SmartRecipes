using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Controls
{
    public static class Controls
    {
        public static Button ActionButton(Icon icon)
        {
            return new Button
            {
                HeightRequest = 64,
                WidthRequest = 64,
                Image = icon.ImageName,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex((string) Application.Current.Resources["primaryColor"]),
                CornerRadius = 32
            };
        }
    }
}