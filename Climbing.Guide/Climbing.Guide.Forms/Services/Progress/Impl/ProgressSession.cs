using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress.Impl {
   public class ProgressSession : Services.Progress.ProgressSession {

      private bool disposed = false;
      private Progress Progress { get; }

      internal ProgressSession(Progress progress) {
         Progress = progress;

         Task.Run(Progress.ShowProgressIndicatorAsync);
      }

      ~ProgressSession() {
         Dispose(false);
      }

      public async Task UpdateProgressAsync(double progress, double total, string message) {
         await Progress.UpdateLoadingProgressAsync(progress, total, message);
      }

      public void Dispose() {
         // Dispose of unmanaged resources.
         Dispose(true);
         // Suppress finalization.
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing) {
         if (!disposed) {
            if (disposing) {
               // Free managed resource
               Task.Run(Progress.HideProgressIndicatorAsync);
            }
            disposed = true;
         }
      }
   }
}
