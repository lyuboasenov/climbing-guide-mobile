using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface IAlerts {
      Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
      Task DisplayAlertAsync(string title, string message, string cancel);
      Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
   }
}