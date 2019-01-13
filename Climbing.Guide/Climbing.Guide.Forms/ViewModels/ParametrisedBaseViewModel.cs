using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ParametrisedBaseViewModel<TParameters> : BaseViewModel, INavigatedAware where TParameters : class  {

      public ParametrisedBaseViewModel() {

      }

      protected virtual Task OnNavigatedToAsync(TParameters parameters) {
         return Task.CompletedTask;
      }

      public new void OnNavigatedTo(INavigationParameters parameters) {
         if (null == parameters || parameters.Count == 0) {
            OnNavigatedToAsync();
         } else {
            var firstParameter = parameters.ElementAt(0) as TParameters;

            if (null == firstParameter) {
               throw new ArgumentException("Parameter type mismatch");
            }

            OnNavigatedToAsync(firstParameter);
         }
      }
   }
}
