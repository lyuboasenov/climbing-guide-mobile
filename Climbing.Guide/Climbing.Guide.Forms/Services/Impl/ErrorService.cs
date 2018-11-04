using Climbing.Guide.Forms.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services {
   public class ErrorService : IErrorService {

      private IAlertService AlertService { get; set; }
      private ILoggingService LoggingService { get; set; }

      public ErrorService(IAlertService alertService, ILoggingService loggingService) {
         AlertService = alertService;
         LoggingService = loggingService;
      }

      private string FormatException(Exception ex) {
         StringBuilder sb = new StringBuilder();

         string indent = string.Empty;
         sb.AppendLine($"====================================================== ERROR: ======================================================");
         while(ex != null) {
            sb.AppendLine($"{indent}Type: {ex.GetType()}");
            sb.AppendLine($"{indent}Message: {ex.Message}");
            sb.AppendLine($"{indent}Message: {ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + indent)}");
            sb.AppendLine($"--------------------------------------------------------------------------------------------------------------------");

            indent += "   ";
            ex = ex.InnerException;
         }
         sb.AppendLine($"=====================================================================================================================");

         return sb.ToString();
      }

      private void LogException(Exception ex) {
         LoggingService.Log(FormatException(ex), Category.Exception, Priority.High);
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public async Task HandleExceptionAsync(Exception ex,
         string errorMessageFormat,
         params object[] errorMessageParams) {

         LogException(ex);

         if (null != AlertService) {
            Device.BeginInvokeOnMainThread(async () => {
               await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               string.Format(errorMessageFormat, errorMessageParams),
               Resources.Strings.Main.Ok);
            });
         }
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public async Task HandleExceptionDetailedAsync(Exception ex,
         string errorMessage,
         string detailedErrorMessageFormat,
         params object[] detailedErrorMessageParams) {

         LogException(ex);

         if (null != AlertService) {
            Device.BeginInvokeOnMainThread(async () => {
               if (await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               errorMessage,
               Resources.Strings.Main.Details_Button, Resources.Strings.Main.Ok)) {
                  await AlertService.DisplayAlertAsync(
                     Resources.Strings.Main.Error_Title,
                     string.Format(detailedErrorMessageFormat, detailedErrorMessageParams),
                     Resources.Strings.Main.Ok);
               }
            });
         }
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public async Task HandleApiCallExceptionAsync(Api.Schemas.ApiCallException ex,
         string errorMessage,
         string detailedErrorMessageFormat) {
         await HandleExceptionDetailedAsync(ex, errorMessage, detailedErrorMessageFormat, Environment.NewLine, ex.StatusCode, ex.Response);
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      public async Task HandleApiCallExceptionAsync(Api.Schemas.ApiCallException ex) {
         await HandleApiCallExceptionAsync(ex,
            Resources.Strings.Main.Communication_Error_Message,
            Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
      }
   }
}
