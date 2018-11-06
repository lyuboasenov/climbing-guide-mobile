using System;

namespace Climbing.Guide.Logging {
   public interface ILogger {
      void Log(string message, Category category = Category.Info, Priority priority = Priority.None);
      void Log(Exception ex);
   }
}