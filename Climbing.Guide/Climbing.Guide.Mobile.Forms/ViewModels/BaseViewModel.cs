using Climbing.Guide.Mobile.Forms.Services;
using Prism.Mvvm;
using Prism.Navigation;
using System.Linq;

namespace Climbing.Guide.Mobile.Forms.ViewModels {
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

      public virtual void Destroy() {

      }
   }
}
