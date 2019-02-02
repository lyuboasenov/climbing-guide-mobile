using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LoadingView : Rg.Plugins.Popup.Pages.PopupPage {
      public LoadingView() {
         InitializeComponent();
      }
   }
}