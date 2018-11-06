using Climbing.Guide.Extensions;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Climbing.Guide.Logging {
   public class DebugLogger : ILogger {
      public DebugLogger() {
      }
      /// <summary>
      /// Write a new log entry with the specified category and priority.
      /// </summary>
      /// <param name="message">Message body to log.</param>
      /// <param name="category">Category of the entry.</param>
      /// <param name="priority">The priority of the entry.</param>
      public void Log(string message, Category category = Category.Info, Priority priority = Priority.None) {
         
         string messageToLog = string.Format(
            CultureInfo.InvariantCulture, @"{{{0:u}}} {1}: {2}. Priority: {3}.", DateTime.Now,
            category.ToString().ToUpper(), message, priority);

         Debug.WriteLine(messageToLog);
      }

      public void Log(Exception ex) {
         this.Log(ex.ToLogMessage(), Category.Exception, Priority.None);
      }
   }
}
