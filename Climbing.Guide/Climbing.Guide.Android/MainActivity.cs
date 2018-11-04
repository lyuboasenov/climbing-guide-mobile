using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Climbing.Guide.Forms;

namespace Climbing.Guide.Droid {
   [Activity(Label = "CG", Icon = "@mipmap/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
      protected override void OnCreate(Bundle savedInstanceState) {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;

         base.Window.RequestFeature(WindowFeatures.ActionBar);
         // Name of the MainActivity theme you had there before.
         // Or you can use global::Android.Resource.Style.ThemeHoloLight
         base.SetTheme(Resource.Style.MainTheme);

         base.OnCreate(savedInstanceState);
         global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

         // Initialize maps
         Xamarin.FormsMaps.Init(this, savedInstanceState);
         LoadApplication(new App());
      }
   }
}