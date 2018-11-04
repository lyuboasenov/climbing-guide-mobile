using Android.App;
using Climbing.Guide.Forms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.Droid.Services.CloseApplication))]

namespace Climbing.Guide.Droid.Services {
   public class CloseApplication : ICloseApplication {
      public void closeApplication() {
         var activity = (Activity)Xamarin.Forms.Forms.Context;
         activity.FinishAffinity();
      }
   }
}