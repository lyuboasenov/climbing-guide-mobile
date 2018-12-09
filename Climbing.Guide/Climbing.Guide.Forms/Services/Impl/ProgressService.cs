using Climbing.Guide.Forms.Events;
using Climbing.Guide.Tasks;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public class ProgressService : IProgressService {

      private Lazy<PopupPage> LoadingView { get; set; }
      private Lazy<PopupPage> ProgressView { get; set; }

      private IEventService EventService { get; set; }
      private IMainThreadTaskRunner MainThreadTaskRunner { get; set; }

      public ProgressService(IEventService eventService, IMainThreadTaskRunner mainThreadTaskRunner) {
         EventService = eventService;
         MainThreadTaskRunner = mainThreadTaskRunner;

         LoadingView = new Lazy<PopupPage>(() => new Views.LoadingView());
         ProgressView = new Lazy<PopupPage>(() => new Views.ProgressView(EventService));
      }

      public async Task ShowLoadingIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(ShowLoadingIndicatorInternal);
      }
      public async Task HideLoadingIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(HideLoadingIndicatorInternal);
      }

      public async Task ShowProgressIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(ShowProgressIndicatorInternal);
      }
      public async Task HideProgressIndicatorAsync() {
         await MainThreadTaskRunner.RunOnUIThreadAsync(HideProgressIndicatorInternal);
      }

      public Task UpdateLoadingProgressAsync(double processed, double total, string message) {
         EventService.GetEvent<ProgressChangedEvent, Events.Payload.ProgressChanged>().
            Publish(new Events.Payload.ProgressChanged() {
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
   }
}
