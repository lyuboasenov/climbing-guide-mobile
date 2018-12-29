using Prism.Mvvm;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigationAware {
      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
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

      public virtual Task OnNavigatedToAsync(params object[] parameters) {
         return Task.CompletedTask;
      }

      public virtual Task OnNavigatingToAsync(params object[] parameters) {
         return Task.CompletedTask;
      }
   }
}
