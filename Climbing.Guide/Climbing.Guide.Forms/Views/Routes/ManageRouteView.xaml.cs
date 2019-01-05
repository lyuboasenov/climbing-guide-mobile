using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ManageRouteView : TabbedPage {
      public ManageRouteView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}