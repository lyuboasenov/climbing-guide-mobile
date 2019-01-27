using Climbing.Guide.Tasks;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress {
   public class Progress : IProgress {

      private Lazy<PopupPage> LoadingView { get; set; }
      private Lazy<Views.ProgressView> ProgressView { get; set; }

      private IMainThreadTaskRunner MainThreadTaskRunner { get; set; }

      public Progress(IMainThreadTaskRunner mainThreadTaskRunner) {
         MainThreadTaskRunner = mainThreadTaskRunner;

         LoadingView = new Lazy<PopupPage>(() => new Views.LoadingView());
         ProgressView = new Lazy<Views.ProgressView>(() => new Views.ProgressView());
      }

      internal async Task ShowLoadingIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(ShowLoadingIndicatorInternal);
      }
      internal async Task HideLoadingIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(HideLoadingIndicatorInternal);
      }

      internal async Task ShowProgressIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(ShowProgressIndicatorInternal);
      }
      internal async Task HideProgressIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(HideProgressIndicatorInternal);
      }

      internal Task UpdateLoadingProgressAsync(double processed, double total, string message) {
         ProgressView.Value.ProgressChanged(message, processed, total);
         return Task.CompletedTask;
      }

      private void ShowLoadingIndicatorInternal() {
         Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(LoadingView.Value);
      }

      private void HideLoadingIndicatorInternal() {
         Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
      }

      private void ShowProgressIndicatorInternal() {
         Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(ProgressView.Value);
      }

      private void HideProgressIndicatorInternal() {
         Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
      }

      public Task<ILoadingSession> CreateLoadingSessionAsync() {
         return Task.FromResult<ILoadingSession>(new LoadingSession(this));
      }

      public Task<IProgressSession> CreateProgressSessionAsync() {
         return Task.FromResult<IProgressSession>(new ProgressSession(this));
      }
   }
}
