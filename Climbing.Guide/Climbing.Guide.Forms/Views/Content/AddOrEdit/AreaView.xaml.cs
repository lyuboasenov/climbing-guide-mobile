using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Content.AddOrEdit {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class AreaView : TabbedPage {
      public AreaView() {
         InitializeComponent();
         Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
      }
   }
}