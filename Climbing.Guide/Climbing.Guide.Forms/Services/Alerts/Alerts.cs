using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Alerts {
   public class Alerts : IAlerts {
      private IPageDialogService InternalPageDialogService { get; }

      public Alerts(IPageDialogService pageDialogService) {
         InternalPageDialogService = pageDialogService;
      }

      public async Task<string> DisplayActionSheetAsync(
         string title, string cancel, string destruction, params string[] buttons) {
         return await InternalPageDialogService.DisplayActionSheetAsync(title, cancel, destruction, buttons);
      }

      public async Task<TItem> DisplayActionSheetAsync<TItem>(
         string title, string cancel, string destruction, Func<TItem, string> buttonTextExtractor, params TItem[] items) {
         var buttons = items.Select(buttonTextExtractor);
         var selectedButton =
            await InternalPageDialogService.DisplayActionSheetAsync(title, cancel, destruction, buttons.ToArray());

         return items.FirstOrDefault(i => buttonTextExtractor(i) == selectedButton);
      }

      public async Task DisplayAlertAsync(string title, string message, string cancel) {
         await InternalPageDialogService.DisplayAlertAsync(title, message, cancel);
      }

      public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel) {
         return await InternalPageDialogService.DisplayAlertAsync(title, message, accept, cancel);
      }
   }
}
