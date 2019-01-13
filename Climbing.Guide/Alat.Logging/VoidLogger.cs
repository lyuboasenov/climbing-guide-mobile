using System;

namespace Alat.Logging {
   /// <summary>
   /// Implementation of <see cref="Logger"/> that does nothing. This
   /// implementation is useful when the application does not need logging
   /// but there are infrastructure pieces that assume there is a logger.
   /// </summary>
   public class VoidLogger : Logger {

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

      public void Debug(string message) {
         
      }

      public void Debug(object obj) {
         
      }

      public void Exception(string message) {
         
      }

      public void Exception(object obj) {
         
      }

      public void Info(string message) {
         
      }

      public void Info(object obj) {
         
      }

      public void Warn(string message) {
         
      }

      public void Warn(object obj) {
         
      }
   }
}
