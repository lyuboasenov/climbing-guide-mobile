using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Exceptions {
   public interface IExceptionHandler {
      Task HandleAsync(Exception ex);
      Task HandleAsync(Exception ex, string message, params object[] messageParameters);
      Task ExecuteErrorHandled(Func<Task> action);
      Task ExecuteErrorHandled(Func<Task> action, string message, params object[] messageParameters);
   }
}
