using System.Threading;
using Climbing.Guide.Forms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.iOS.Services.CloseApplication))]

namespace Climbing.Guide.iOS.Services {
   public class CloseApplication : ICloseApplication {
      public void closeApplication() {
         Thread.CurrentThread.Abort();
      }
   }
}