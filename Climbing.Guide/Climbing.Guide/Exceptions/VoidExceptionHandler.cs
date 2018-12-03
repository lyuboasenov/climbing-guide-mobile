using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Exceptions {
   public class VoidExceptionHandler : IExceptionHandler {
      public Task HandleAsync(Exception ex) {
         return Task.CompletedTask;
      }

      public Task HandleAsync(Exception ex, string message, params object[] messageParameters) {
         return Task.CompletedTask;
      }
   }
}
