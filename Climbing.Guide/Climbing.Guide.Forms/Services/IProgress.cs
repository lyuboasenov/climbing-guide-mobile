using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface IProgress {
      Task ShowLoadingIndicatorAsync();
      Task HideLoadingIndicatorAsync();

      Task ShowProgressIndicatorAsync();
      Task HideProgressIndicatorAsync();

      Task UpdateLoadingProgressAsync(double progress, double total, string message);
   }
}
