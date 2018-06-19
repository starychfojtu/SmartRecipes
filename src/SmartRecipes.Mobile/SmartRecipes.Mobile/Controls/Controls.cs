using LanguageExt;
using SmartRecipes.Mobile.Extensions;
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
                BackgroundColor = Color.White,
                CornerRadius = 16
            };
        }
        
        public static MenuItem MenuItem(UserAction<Unit> action, bool destructive = false)
        {
            var item = new MenuItem
            {
                IsDestructive = destructive,
                Icon = action.Icon.ImageName
            }; 
            return item.Tee(i => i.Clicked += async (sender, e) => await UserMessage.PopupAction(() => action.Action(Unit.Default)));
        }
    }
}