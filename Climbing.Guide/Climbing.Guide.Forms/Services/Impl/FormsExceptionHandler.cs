using Climbing.Guide.Exceptions;
using Climbing.Guide.Logging;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services {
   public class FormsExceptionHandler : IExceptionHandler {

      private IAlertService AlertService { get; set; }
      private ILogger LoggingService { get; set; }

      public FormsExceptionHandler(IAlertService alertService, ILogger loggingService) {
         AlertService = alertService;
         LoggingService = loggingService;
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      public Task HandleAsync(Exception ex) {

         LoggingService.Log(ex);

         if (null != AlertService) {
            Device.BeginInvokeOnMainThread(async () => {
               await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               Resources.Strings.Main.Error_Mesage,
               Resources.Strings.Main.Ok);
            });
         }

         return Task.CompletedTask;
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessageFormat">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public Task HandleAsync(Exception ex,
         string errorMessageFormat,
         params object[] errorMessageParams) {

         LoggingService.Log(ex);

         if (null != AlertService) {
            Device.BeginInvokeOnMainThread(async () => {
               await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               string.Format(errorMessageFormat, errorMessageParams),
               Resources.Strings.Main.Ok);
            });
         }

         return Task.CompletedTask;
      }

      ///// <summary>
      ///// Displays error message and detailed error message if selected.
      ///// </summary>
      ///// <param name="ex">The communication exception to handle</param>
      ///// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      ///// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      //public async Task HandleExceptionDetailedAsync(Exception ex,
      //   string errorMessage,
      //   string detailedErrorMessageFormat,
      //   params object[] detailedErrorMessageParams) {

      //   LoggingService.Log(ex);

      //   if (null != AlertService) {
      //      Device.BeginInvokeOnMainThread(async () => {
      //         if (await AlertService.DisplayAlertAsync(
      //         Resources.Strings.Main.Error_Title,
      //         errorMessage,
      //         Resources.Strings.Main.Details_Button, Resources.Strings.Main.Ok)) {
      //            await AlertService.DisplayAlertAsync(
      //               Resources.Strings.Main.Error_Title,
      //               string.Format(detailedErrorMessageFormat, detailedErrorMessageParams),
      //               Resources.Strings.Main.Ok);
      //         }
      //      });
      //   }
      //}

      ///// <summary>
      ///// Displays error message and detailed error message if selected.
      ///// </summary>
      ///// <param name="ex">The communication exception to handle</param>
      ///// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      ///// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      //public async Task HandleApiCallExceptionAsync(Api.Schemas.ApiCallException ex,
      //   string errorMessage,
      //   string detailedErrorMessageFormat) {
      //   await HandleExceptionDetailedAsync(ex, errorMessage, detailedErrorMessageFormat, System.Environment.NewLine, ex.StatusCode, ex.Response);
      //}

      ///// <summary>
      ///// Displays error message and detailed error message if selected.
      ///// </summary>
      ///// <param name="ex">The communication exception to handle</param>
      //public async Task HandleApiCallExceptionAsync(Api.Schemas.ApiCallException ex) {
      //   await HandleApiCallExceptionAsync(ex,
      //      Resources.Strings.Main.Communication_Error_Message,
      //      Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
      //}
   }
}
