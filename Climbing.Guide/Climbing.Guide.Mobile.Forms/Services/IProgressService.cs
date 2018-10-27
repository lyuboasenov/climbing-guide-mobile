using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.Services {
   public interface IProgressService {
      Task ShowLoadingIndicatorAsync();
      Task HideLoadingIndicatorAsync();

      Task ShowProgressIndicatorAsync();
      Task HideProgressIndicatorAsync();

      Task UpdateLoadingProgressAsync(double progress, double total, string message);
   }
}
