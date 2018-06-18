using Xamarin.Forms;

namespace SmartRecipes.Mobile.Controls
{
    public class CardView : Frame
    {
        public CardView()
        {
            Padding = 0;

            if (Device.RuntimePlatform == Device.iOS)
            {
                HasShadow = false;
                BorderColor = Color.Transparent;
                BackgroundColor = Color.Transparent;
            }
        }
    }
}
