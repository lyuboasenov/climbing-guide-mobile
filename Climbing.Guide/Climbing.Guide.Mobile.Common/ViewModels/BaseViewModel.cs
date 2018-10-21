using Climbing.Guide.Mobile.Common.Services;
using FreshMvvm;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : FreshBasePageModel {
      public IRestApiClient Client => ServiceLocator.Get<IRestApiClient>();

      internal INavigationService Navigation => ServiceLocator.Get<INavigationService>();

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
         InitializeCommands();
      }

      protected virtual void InitializeCommands() {

      }

      protected T GetService<T>() where T : class {
         return ServiceLocator.Get<T>();
      }
   }
}
