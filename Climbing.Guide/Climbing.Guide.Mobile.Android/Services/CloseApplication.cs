using Android.App;
using Climbing.Guide.Mobile.Common.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.Mobile.Droid.Services.CloseApplication))]

namespace Climbing.Guide.Mobile.Droid.Services {
   public class CloseApplication : ICloseApplication {
      public void closeApplication() {
         var activity = (Activity)Forms.Context;
         activity.FinishAffinity();
      }
   }
}