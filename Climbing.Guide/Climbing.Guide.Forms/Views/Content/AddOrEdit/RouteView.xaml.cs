using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Content.AddOrEdit {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class RouteView : TabbedPage {
      public RouteView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}