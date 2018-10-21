using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.Services {
   public class AlertService : IAlertService {
      public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons) {
         return await App.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
      }

      public async Task DisplayAlertAsync(string title, string message, string cancel) {
         await App.Current.MainPage.DisplayAlert(title, message, cancel);
      }

      public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel) {
         return await App.Current.MainPage.DisplayAlert(title, message, accept, cancel);
      }
   }
}
