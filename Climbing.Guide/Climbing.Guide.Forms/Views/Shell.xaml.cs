using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Shell : MasterDetailPage, IMasterDetailPageOptions {
      public Shell() {
         InitializeComponent();
      }

      public bool IsPresentedAfterNavigation {
         get { return false; }
      }
   }
}