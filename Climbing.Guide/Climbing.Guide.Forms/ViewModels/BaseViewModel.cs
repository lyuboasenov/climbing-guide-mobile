using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : INavigatedAware, INotifyPropertyChanged {
      public event PropertyChangedEventHandler PropertyChanged;
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

      /// <summary>
      /// Raises this object's PropertyChanged event.
      /// </summary>
      /// <param name="propertyName">Name of the property used to notify listeners. This
      /// value is optional and can be provided automatically when invoked from compilers
      /// that support <see cref="CallerMemberNameAttribute"/>.</param>
      protected void RaisePropertyChanged([CallerMemberName]string propertyName = null) {
         OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
      }

      /// <summary>
      /// Raises this object's PropertyChanged event.
      /// </summary>
      /// <param name="args">The PropertyChangedEventArgs</param>
      protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) {
         PropertyChanged?.Invoke(this, args);
      }
   }
}
