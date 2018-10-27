using Android.App;
using Climbing.Guide.Mobile.Forms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.Mobile.Droid.Services.CloseApplication))]

namespace Climbing.Guide.Mobile.Droid.Services {
   public class CloseApplication : ICloseApplication {
      public void closeApplication() {
         var activity = (Activity)Xamarin.Forms.Forms.Context;
         activity.FinishAffinity();
      }
   }
}