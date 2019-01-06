using Prism.Mvvm;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigatedAware {
      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
      }

      public virtual Task OnNavigatedToAsync(params object[] parameters) {
         return Task.CompletedTask;
      }

      public virtual Task OnNavigatedFromAsync(params object[] parameters) {
         return Task.CompletedTask;
      }
      
      public virtual void OnNavigatedFrom(INavigationParameters parameters) {
         OnNavigatedFromAsync(parameters.Select(p => p.Value).ToArray()).
            ContinueWith((task) => {
               if (task.IsFaulted) {
                  throw task.Exception;
               }
            });
      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {
         OnNavigatedToAsync(parameters.Select(p => p.Value).ToArray()).
            ContinueWith((task) => {
               if (task.IsFaulted) {
                  throw task.Exception;
               }
            });
      }
   }
}
