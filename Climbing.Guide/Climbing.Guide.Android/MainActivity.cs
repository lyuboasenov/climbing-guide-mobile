using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Climbing.Guide.Forms;
using Plugin.CurrentActivity;
using Plugin.Permissions;

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
         Plugin.Iconize.Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                               .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule())
                               .With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule());
         Plugin.Iconize.Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);
         CrossCurrentActivity.Current.Init(this, savedInstanceState);
         Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
         Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code
         Xamarin.Forms.Forms.Init(this, savedInstanceState);

         // Initialize maps
         Xamarin.FormsMaps.Init(this, savedInstanceState);
         LoadApplication(new App());
      }

      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults) {
         PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }
   }
}