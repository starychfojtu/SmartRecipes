using SlideOverKit;
using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
	public class NavigationDrawer : SlideMenuView
	{
		public NavigationDrawer ()
		{
			Content = new StackLayout {
				Children = {
					new Label { Text = "I am navigation page" }
				}
			};
		}
	}
}