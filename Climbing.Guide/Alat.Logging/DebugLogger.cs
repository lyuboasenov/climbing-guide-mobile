using System;
using System.Globalization;

namespace Alat.Logging {
   public class DebugLogger : Logger {
      private ObjectFormatter ObjectFormatter { get; }
      private MessageFormatter MessageFormatter { get; }

      public DebugLogger() : this(new DefaultLogMessageFormatter(), new ToStringFormatter()) {
      }

      public DebugLogger(MessageFormatter messageFormatter, ObjectFormatter objectFormatter) {
         MessageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));
         ObjectFormatter = objectFormatter ?? throw new ArgumentNullException(nameof(objectFormatter));
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

         System.Diagnostics.Debug.WriteLine(messageToLog);
      }

      public void Debug(string message) {
         Log(message, Category.Debug);
      }

      public void Debug(object obj) {
         Debug(ObjectFormatter.Format(obj));
      }

      public void Exception(string message) {
         Log(message, Category.Exception);
      }

      public void Exception(object obj) {
         Exception(ObjectFormatter.Format(obj));
      }

      public void Info(string message) {
         Log(message, Category.Info);
      }

      public void Info(object obj) {
         Info(ObjectFormatter.Format(obj));
      }

      public void Warn(string message) {
         Log(message, Category.Warn);
      }

      public void Warn(object obj) {
         Warn(ObjectFormatter.Format(obj));
      }
   }
}
