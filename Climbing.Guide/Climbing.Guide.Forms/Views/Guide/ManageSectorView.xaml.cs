using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Guide {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ManageSectorView : TabbedPage {
      public ManageSectorView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}