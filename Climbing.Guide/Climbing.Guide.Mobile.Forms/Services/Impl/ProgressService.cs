using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.Services {
   public abstract class ProgressService : IProgressService {

      private Page progressView;
      private Page loadingView;

      protected Page LoadingView {
         get {
            if (null == loadingView) {
               loadingView = new Views.LoadingIndicatorView();
            }

            return loadingView;
         }
      }

      protected Page ProgressView {
         get {
            if (null == progressView) {
               progressView = new Views.ProgressIndicatorView();
            }

            return progressView;
         }
      }

      protected abstract void ShowLoadingIndicatorInternal();
      protected abstract void HideLoadingIndicatorInternal();

      protected abstract void ShowProgressIndicatorInternal();
      protected abstract void HideProgressIndicatorInternal();

      protected abstract void UpdateLoadingProgressInternal(decimal progress, decimal total, string message);

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
      public async Task ShowLoadingIndicatorAsync() {

         Device.BeginInvokeOnMainThread(ShowLoadingIndicatorInternal);
      }
      public async Task HideLoadingIndicatorAsync() {
         Device.BeginInvokeOnMainThread(HideLoadingIndicatorInternal);
      }

      public async Task ShowProgressIndicatorAsync() {
         Device.BeginInvokeOnMainThread(ShowProgressIndicatorInternal);
      }
      public async Task HideProgressIndicatorAsync() {
         Device.BeginInvokeOnMainThread(HideProgressIndicatorInternal);
      }

      public async Task UpdateLoadingProgressAsync(double progress, double total, string message) {
         Device.BeginInvokeOnMainThread(() => {
            (ProgressView as Views.ProgressIndicatorView).ProgressBar.Progress = progress / total;
            (ProgressView as Views.ProgressIndicatorView).MessageLabel.Text = message;
         });
      }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
   }
}
