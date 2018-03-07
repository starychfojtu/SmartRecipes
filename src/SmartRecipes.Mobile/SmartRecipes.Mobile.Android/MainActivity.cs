using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using RoundedBoxView.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;

namespace SmartRecipes.Mobile.Droid
{
    [Activity(Label = "SmartRecipes.Mobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            CachedImageRenderer.Init(enableFastRenderer: true);
            RoundedBoxViewRenderer.Init();

            LoadApplication(new App());
        }
    }
}

