using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Guide {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ManageAreaView : TabbedPage {
      public ManageAreaView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}