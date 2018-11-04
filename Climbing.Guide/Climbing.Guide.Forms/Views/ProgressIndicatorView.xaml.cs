using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ProgressIndicatorView : ContentPage {
      public ProgressIndicatorView() {
         InitializeComponent();
      }

      public Label MessageLabel {
         get {
            return message;
         }
      }

      public ProgressBar ProgressBar {
         get {
            return progressBar;
         }
      }
   }
}