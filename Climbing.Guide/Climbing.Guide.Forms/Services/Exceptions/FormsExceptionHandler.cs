using Alat.Logging;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services.Exceptions {
   public class FormsExceptionHandler : IExceptionHandler {
      private Alerts.Alerts AlertService { get; }
      private ILogger LoggingService { get; }

      public FormsExceptionHandler(Alerts.Alerts alertService, ILogger loggingService) {
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
      /// <param name="message">Error message to be displayed in a alert window.</param>
      public Task HandleAsync(Exception ex,
         string message,
         params object[] messageParameters) {
         LoggingService.Log(ex);

         if (null != AlertService) {
            Device.BeginInvokeOnMainThread(async () => {
               await AlertService.DisplayAlertAsync(
               Guide.Forms.Resources.Strings.Main.Error_Title,
               string.Format(message, messageParameters),
               Guide.Forms.Resources.Strings.Main.Ok);
            });
         }

         return Task.CompletedTask;
      }

      public async Task ExecuteErrorHandled(Func<Task> action) {
         try {
            await action();
         } catch (Exception ex) {
            await HandleAsync(ex);
         }
      }

      public async Task ExecuteErrorHandled(Func<Task> action, string message, params object[] messageParameters) {
         try {
            await action();
         } catch (Exception ex) {
            await HandleAsync(ex, message, messageParameters);
         }
      }
   }
}
