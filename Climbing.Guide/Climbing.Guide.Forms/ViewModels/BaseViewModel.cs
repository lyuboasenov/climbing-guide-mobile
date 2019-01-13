using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigatedAware {
      public BaseViewModel Parent { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
      }

      protected virtual Task OnNavigatedToAsync() {
         return Task.CompletedTask;
      }
      
      public void OnNavigatedFrom(INavigationParameters parameters) { }

      public void OnNavigatedTo(INavigationParameters parameters) {
         if (null != parameters && parameters.Count > 0) {
            throw new ArgumentException("Parameters passed, but non were expected.");
         }

         OnNavigatedToAsync();
      }
   }
}
