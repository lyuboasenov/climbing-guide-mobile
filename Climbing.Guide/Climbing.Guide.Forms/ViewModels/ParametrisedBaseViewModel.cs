using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ParametrisedBaseViewModel<TParameters> : BaseViewModel, INavigatedAware where TParameters : class  {
      protected virtual Task OnNavigatedToAsync(TParameters parameters) {
         return Task.CompletedTask;
      }

      public new void OnNavigatedTo(INavigationParameters parameters) {
         if (null == parameters || parameters.Count == 0) {
            OnNavigatedToAsync();
         } else {
            if (!parameters.Any()) {
               throw new ArgumentNullException(nameof(parameters));
            }

            var firstParameter = parameters.ElementAtOrDefault(0).Value;

            if (null == firstParameter || !(firstParameter is TParameters)) {
               throw new ArgumentException(
                  $"Parameter type mismatch. Expected {typeof(TParameters).FullName}," +
                  $" bug got {firstParameter?.GetType().FullName}");
            }

            OnNavigatedToAsync(firstParameter as TParameters);
         }
      }
   }
}
