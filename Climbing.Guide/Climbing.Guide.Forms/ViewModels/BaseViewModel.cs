using Climbing.Guide.Core.Api;
using Climbing.Guide.Services;
using Climbing.Guide.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigationAware {

      protected ITaskRunner TaskRunner { get; } = GetService<ITaskRunner>();

      public IApiClient Client => GetService<IApiClient>();
      protected Services.INavigationService Navigation => GetService<Services.INavigationService>();
      protected IErrorService Errors => GetService<IErrorService>();

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }
      public IEnumerable<string> ValidationErrors { get; set; }

      public BaseViewModel() {
         InitializeCommands();
      }

      protected virtual void InitializeCommands() {

      }

      protected virtual Task InitializeViewModel() {
         return Task.CompletedTask;
      }

      protected static T GetService<T>() where T : class {
         return IoC.Container.Get<T>();
      }

      public void OnNavigatedFrom(INavigationParameters parameters) {
         OnNavigatedFromAsync(parameters.Select(p => p.Value));
      }

      public void OnNavigatedTo(INavigationParameters parameters) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
         OnNavigatedToAsync(parameters.Select(p => p.Value).ToArray());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }

      public void OnNavigatingTo(INavigationParameters parameters) {
         OnNavigatingToAsync(parameters.Select(p => p.Value));
      }

      public virtual Task OnNavigatedFromAsync(params object[] parameters) {
         return Task.CompletedTask;
      }

      public async virtual Task OnNavigatedToAsync(params object[] parameters) {
         await InitializeViewModel();
      }

      public virtual Task OnNavigatingToAsync(params object[] parameters) {
         return Task.CompletedTask;
      }
   }
}
