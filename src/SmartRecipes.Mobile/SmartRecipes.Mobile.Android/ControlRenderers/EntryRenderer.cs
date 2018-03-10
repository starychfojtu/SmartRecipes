using Android.Views;
using SmartRecipes.Mobile.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(SmartRecipes.Mobile.Droid.ControlRenderers.EntryRenderer))]
namespace SmartRecipes.Mobile.Droid.ControlRenderers
{
    public class EntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        public EntryRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Gravity = GravityFlags.CenterVertical;
                Control.SetPadding(16, 16, 16, 16);
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
