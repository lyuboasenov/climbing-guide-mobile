using System;

namespace Alat.Logging {
   public interface Logger {
      void Log(string message, Category category = Category.Info, Priority priority = Priority.None);
      void Debug(string message);
      void Exception(string message);
      void Info(string message);
      void Warn(string message);

      void Debug(object obj);
      void Exception(object obj);
      void Info(object obj);
      void Warn(object obj);
   }
}