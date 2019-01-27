using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ProgressView : Rg.Plugins.Popup.Pages.PopupPage {
      public ProgressView() {
         InitializeComponent();
      }

      public void ProgressChanged(string message, double processed, double total) {
         MessageLabel.Text = message;
         ProgressBar.Progress = processed / total;
      }
   }
}