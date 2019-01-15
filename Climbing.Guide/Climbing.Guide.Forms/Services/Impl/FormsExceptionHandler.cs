using Climbing.Guide.Exceptions;
using Climbing.Guide.Logging;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services.Impl {
   public class FormsExceptionHandler : IExceptionHandler {

      private Alerts AlertService { get; set; }
      private Logger LoggingService { get; set; }

      public FormsExceptionHandler(Alerts alertService, Logger loggingService) {
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
               Guide.Forms.Resources.Strings.Main.Error_Title,
               Guide.Forms.Resources.Strings.Main.Error_Mesage,
               Guide.Forms.Resources.Strings.Main.Ok);
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
               Guide.Forms.Resources.Strings.Main.Error_Title,
               string.Format(errorMessageFormat, errorMessageParams),
               Guide.Forms.Resources.Strings.Main.Ok);
            });
         }

         return Task.CompletedTask;
      }
   }
}
