using Climbing.Guide.Forms.Events;
using Climbing.Guide.Tasks;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress.Impl {
   public class Progress : Services.Progress.Progress {

      private Lazy<PopupPage> LoadingView { get; set; }
      private Lazy<PopupPage> ProgressView { get; set; }

      private Events EventService { get; set; }
      private IMainThreadTaskRunner MainThreadTaskRunner { get; set; }

      public Progress(Events eventService, IMainThreadTaskRunner mainThreadTaskRunner) {
         EventService = eventService;
         MainThreadTaskRunner = mainThreadTaskRunner;

         LoadingView = new Lazy<PopupPage>(() => new Views.LoadingView());
         ProgressView = new Lazy<PopupPage>(() => new Views.ProgressView(EventService));
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
         EventService.GetEvent<ProgressChangedEvent, Guide.Forms.Events.Payload.ProgressChanged>().
            Publish(new Guide.Forms.Events.Payload.ProgressChanged() {
               Message = message,
               Processed = processed,
               Total = total
            });
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

      public Task<Services.Progress.LoadingSession> CreateLoadingSessionAsync() {
         return Task.FromResult<Services.Progress.LoadingSession>(new LoadingSession(this));
      }

      public Task<Services.Progress.ProgressSession> CreateProgressSessionAsync() {
         return Task.FromResult<Services.Progress.ProgressSession>(new ProgressSession(this));
      }
   }
}
