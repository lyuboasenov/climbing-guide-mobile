using System;

namespace Climbing.Guide.Logging {
   /// <summary>
   /// Implementation of <see cref="ILogger"/> that does nothing. This
   /// implementation is useful when the application does not need logging
   /// but there are infrastructure pieces that assume there is a logger.
   /// </summary>
   public class VoidLogger : ILogger {

      public VoidLogger() {
      }
      /// <summary>
      /// Write a new log entry with the specified category and priority.
      /// </summary>
      /// <param name="message">Message body to log.</param>
      /// <param name="category">Category of the entry.</param>
      /// <param name="priority">The priority of the entry.</param>
      public void Log(string message, Category category = Category.Info, Priority priority = Priority.None) {

      }

      public void Log(Exception ex) {
      }
   }
}
