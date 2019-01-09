using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface Alerts {
      Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
      Task<TItem> DisplayActionSheetAsync<TItem>(string title, string cancel, string destruction, Func<TItem, string> buttonTextExtractor, params TItem[] items);
      Task DisplayAlertAsync(string title, string message, string cancel);
      Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
   }
}