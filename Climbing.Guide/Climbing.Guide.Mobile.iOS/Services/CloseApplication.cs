using System.Threading;
using Climbing.Guide.Mobile.Common.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.Mobile.iOS.Services.CloseApplication))]

namespace Climbing.Guide.Mobile.iOS.Services {
   public class CloseApplication : ICloseApplication {
      public void closeApplication() {
         Thread.CurrentThread.Abort();
      }
   }
}