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
                HeightRequest = 32,
                WidthRequest = 32,
                Image = icon.ImageName,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = (Color) Application.Current.Resources["accentColor"],
                CornerRadius = 16
            };
        }
    }
}