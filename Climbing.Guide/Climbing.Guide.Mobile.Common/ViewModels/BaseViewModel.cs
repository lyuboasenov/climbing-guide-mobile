using Climbing.Guide.Mobile.Common.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigationAware, IDestructible {
      public IRestApiClient Client => IoC.Container.Get<IRestApiClient>();

      protected Services.INavigationService NavigationService => IoC.Container.Get<Services.INavigationService>();

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
         InitializeCommands();
      }

      protected virtual void InitializeCommands() {

      }

      protected T GetService<T>() where T : class {
         return IoC.Container.Get<T>();
      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {

      }

      public virtual void OnNavigatingTo(INavigationParameters parameters) {

      }

      public virtual void Destroy() {

      }
   }
}
