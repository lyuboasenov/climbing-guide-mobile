using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Content {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ManageRouteView : TabbedPage {
      public ManageRouteView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}