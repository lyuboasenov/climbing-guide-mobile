using Climbing.Guide.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.UWP.Services.CloseApplication))]

namespace Climbing.Guide.UWP.Services {
   class CloseApplication : ICloseApplication {
      public void closeApplication() {
      }
   }
}
