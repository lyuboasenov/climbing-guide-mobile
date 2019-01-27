using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress {
   public class LoadingSession : ILoadingSession {

      private bool disposed = false;
      private Progress Progress { get; }

      internal LoadingSession(Progress progress) {
         Progress = progress;

         Task.Run(Progress.ShowLoadingIndicatorAsync);
      }

      ~LoadingSession() {
         Dispose(false);
      }

      public void Dispose() {
         // Dispose of unmanaged resources.
         Dispose(true);
         // Suppress finalization.
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing) {
         if (!disposed) {
            Task.Run(Progress.HideLoadingIndicatorAsync);
            if (disposing) {
               // Free managed resource
            }
            disposed = true;
         }
      }
   }
}
