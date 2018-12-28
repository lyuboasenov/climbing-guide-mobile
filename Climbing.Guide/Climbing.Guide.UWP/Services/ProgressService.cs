using Climbing.Guide.Forms.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Climbing.Guide.UWP.Services.ProgressService))]

namespace Climbing.Guide.UWP.Services {
   class ProgressService : IProgress {
      public Task HideLoadingIndicatorAsync() {
         return Task.CompletedTask;
      }

      public Task HideProgressIndicatorAsync() {
         return Task.CompletedTask;
      }

      public Task ShowLoadingIndicatorAsync() {
         return Task.CompletedTask;
      }

      public Task ShowProgressIndicatorAsync() {
         return Task.CompletedTask;
      }

      public Task UpdateLoadingProgressAsync(double progress, double total, string message) {
         return Task.CompletedTask;
      }
   }
}
