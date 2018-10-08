using Climbing.Guide.Core.API;
using FreshMvvm;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : FreshBasePageModel {
      protected IRestApiClient RestClient {
         get {
            return RestApiClient.Instance;
         }
      }

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }
   }
}
