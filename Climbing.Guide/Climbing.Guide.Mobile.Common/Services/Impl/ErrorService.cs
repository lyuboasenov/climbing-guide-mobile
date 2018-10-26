using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.Services {
   public class ErrorService : IErrorService {

      private IAlertService alertService;
      // Cross servivce dependencies should be lazy loaded in order to overcome
      // initialization sequencing.
      private IAlertService AlertService {
         get {
            if (null == alertService) {
               alertService = IoC.Container.Get<IAlertService>();
            }

            return alertService;
         }
      }

      public void LogException(Exception ex) {
         string indent = string.Empty;
         Console.WriteLine($"====================================================== ERROR: ======================================================");
         while(ex != null) {
            Console.WriteLine($"{indent}Type: {ex.GetType()}");
            Console.WriteLine($"{indent}Message: {ex.Message}");
            Console.WriteLine($"{indent}Message: {ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + indent)}");
            Console.WriteLine($"--------------------------------------------------------------------------------------------------------------------");

            indent += "   ";
            ex = ex.InnerException;
         }
         Console.WriteLine($"Error occured while initializing the application: {ex.Message}");
         Console.WriteLine($"=====================================================================================================================");
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
            await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               string.Format(errorMessageFormat, errorMessageParams),
               Resources.Strings.Main.Ok);
         }
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public async Task HandleExceptionAsync(Exception ex,
         string errorMessage,
         string detailedErrorMessageFormat,
         params object[] detailedErrorMessageParams) {

         LogException(ex);

         if (null != AlertService) {
            if (await AlertService.DisplayAlertAsync(
               Resources.Strings.Main.Error_Title,
               errorMessage,
               Resources.Strings.Main.Ok, Resources.Strings.Main.Details_Button)) {
               await AlertService.DisplayAlertAsync(
                  Resources.Strings.Main.Error_Title,
                  string.Format(detailedErrorMessageFormat, detailedErrorMessageParams),
                  Resources.Strings.Main.Ok);
            }
         }
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      /// <param name="errorMessage">Error message to be displayed in a alert window.</param>
      /// <param name="detailedErrorMessageFormat">Format string to be used in detailed message forming, to be displayed in detailed alert window.</param>
      public async Task HandleRestApiCallExceptionAsync(Core.API.Schemas.RestApiCallException ex,
         string errorMessage,
         string detailedErrorMessageFormat) {
         await HandleExceptionAsync(ex, errorMessage, detailedErrorMessageFormat, Environment.NewLine, ex.StatusCode, ex.Response);
      }

      /// <summary>
      /// Displays error message and detailed error message if selected.
      /// </summary>
      /// <param name="ex">The communication exception to handle</param>
      public async Task HandleRestApiCallExceptionAsync(Core.API.Schemas.RestApiCallException ex) {
         await HandleRestApiCallExceptionAsync(ex,
            Resources.Strings.Main.Communication_Error_Message,
            Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
      }
   }
}
