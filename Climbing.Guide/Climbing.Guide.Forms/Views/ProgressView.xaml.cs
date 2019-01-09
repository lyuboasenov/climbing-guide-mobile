using Climbing.Guide.Forms.Events.Payload;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ProgressView : Rg.Plugins.Popup.Pages.PopupPage {
      public ProgressView(Services.Events errorService) {
         InitializeComponent();
         errorService.GetEvent<Events.ProgressChangedEvent, Events.Payload.ProgressChanged>().Subscribe(ProgressChanged);
      }

      private void ProgressChanged(ProgressChanged obj) {
         MessageLabel.Text = obj.Message;
         ProgressBar.Progress = obj.Processed / obj.Total;
      }
   }
}