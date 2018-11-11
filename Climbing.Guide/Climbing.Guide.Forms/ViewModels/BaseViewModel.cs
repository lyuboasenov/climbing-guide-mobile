using Climbing.Guide.Core.Api;
using Climbing.Guide.Services;
using Climbing.Guide.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;

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

      protected static T GetService<T>() where T : class {
         return IoC.Container.Get<T>();
      }

      public void OnNavigatedFrom(INavigationParameters parameters) {
         OnNavigatedFrom(parameters.Select(p => p.Value));
      }

      public void OnNavigatedTo(INavigationParameters parameters) {
         OnNavigatedTo(parameters.Select(p => p.Value).ToArray());
      }

      public void OnNavigatingTo(INavigationParameters parameters) {
         OnNavigatingTo(parameters.Select(p => p.Value));
      }

      public virtual void OnNavigatedFrom(params object[] parameters) {

      }

      public virtual void OnNavigatedTo(params object[] parameters) {

      }

      public virtual void OnNavigatingTo(params object[] parameters) {

      }
   }
}
