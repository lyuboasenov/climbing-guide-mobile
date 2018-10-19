using Climbing.Guide.Mobile.Common.Services;
using FreshMvvm;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : FreshBasePageModel {
      protected IRestApiClient Client => DependencyService.Get<IRestApiClient>();

      internal INavigationService Navigation => DependencyService.Get<INavigationService>();

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      protected async Task HandleRestApiCallException(Core.API.Schemas.RestApiCallException ex,
         string errorMessage,
         string detailedErrorMessageFormat) {
         if (await CurrentPage.DisplayAlert(
               Resources.Strings.Main.Error_Title,
               errorMessage,
               Resources.Strings.Main.Ok, Resources.Strings.Main.Details_Button)) {
            await CurrentPage.DisplayAlert(
               Resources.Strings.Main.Error_Title,
               string.Format(detailedErrorMessageFormat, Environment.NewLine, ex.StatusCode, ex.Response),
               Resources.Strings.Main.Ok);
         }
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      protected async Task HandleRestApiCallException(Core.API.Schemas.RestApiCallException ex) {
         await HandleRestApiCallException(ex,
            Resources.Strings.Main.Communication_Error_Message,
            Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
      }
   }
}
