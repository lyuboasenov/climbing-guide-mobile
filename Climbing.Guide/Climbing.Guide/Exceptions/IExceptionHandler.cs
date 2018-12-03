using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Exceptions {
   public interface IExceptionHandler {
      Task HandleAsync(Exception ex);
      Task HandleAsync(Exception ex, string message, params object[] messageParameters);
   }
}
