using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using RoundedBoxView.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;
using Java.Lang;
using System;
using System.Net;

namespace SmartRecipes.Mobile.Droid
{
    [Activity(Label = "SmartRecipes.Mobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CachedImageRenderer.Init(enableFastRenderer: true);
            RoundedBoxViewRenderer.Init();

            LoadApplication(new App());
        }
    }
}

