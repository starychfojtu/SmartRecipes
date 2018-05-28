using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using FFImageLoading.Forms.Touch;
using RoundedBoxView.Forms.Plugin.iOSUnified;
using Xamarin.Forms.Platform.iOS;
using SuaveControls.FloatingActionButton.iOS.Renderers;
using System.Net;

namespace SmartRecipes.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            global::Xamarin.Forms.Forms.Init();
            SlideOverKit.iOS.SlideOverKit.Init();
            CachedImageRenderer.Init();
            RoundedBoxViewRenderer.Init();
            FloatingActionButtonRenderer.InitRenderer();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
