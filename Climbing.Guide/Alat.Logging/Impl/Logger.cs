using Alat.Logging.ToLogEntryDataConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Alat.Logging.Impl {
   public class Logger : Logging.Logger {
      private Level DefaultLogLevel => Level.Trace;

      private LoggerSettings Settings { get; set; }

      public Logger(LoggerSettings settings) {
         Settings = settings;
      }

      public void Debug(string message) {
         LogObject(message, Level.Debug);
      }

      public void Debug(object obj) {
         LogObject(obj, Level.Debug);
      }

      public void Error(string message) {
         LogObject(message, Level.Error);
      }

      public void Error(object obj) {
         LogObject(obj, Level.Error);
      }

      public void Fatal(string message) {
         LogObject(message, Level.Fatal);
      }

      public void Fatal(object obj) {
         LogObject(obj, Level.Fatal);
      }

      public void Info(string message) {
         LogObject(message, Level.Info);
      }

      public void Info(object obj) {
         LogObject(obj, Level.Info);
      }

      public void Log(object obj) {
         LogObject(obj, DefaultLogLevel);
      }

      public void Log(object obj, Level level) {
         LogObject(obj, level);
      }

      public void Log(string message) {
         LogObject(message, DefaultLogLevel);
      }

      public void Log(string message, Level level) {
         LogObject(message, level);
      }

      public void Warn(string message) {
         LogObject(message, Level.Warn);
      }

      public void Warn(object obj) {
         LogObject(obj, Level.Warn);
      }

      private void LogObject(object obj, Level level) {
         if (null == obj) {
            throw new ArgumentNullException(nameof(obj));
         }

         if (obj is string && string.IsNullOrEmpty((string)obj)) {
            throw new ArgumentNullException(nameof(obj));
         }

         if (null == level) {
            throw new ArgumentNullException(nameof(level));
         }

         var logEntry = CreateLogEntry(obj, level);

         foreach (var appender in Settings.LoggerAppenders) {
            appender.Write(logEntry);
         }
      }


      private LogEntry CreateLogEntry(object obj, Level level) {
         var type = obj.GetType();
         var toLogEntryDataConverter = Settings.ToLogEntryDataConverters.FirstOrDefault(p => p.Key == type);
         LogEntryData data = null;
         if (default(KeyValuePair<Type, ToLogEntryDataConverter>).Equals(toLogEntryDataConverter)) {
            data = new LogEntryData(obj.ToString()); 
         } else {
            data = toLogEntryDataConverter.Value.Convert(obj);
         }

         string stackTrace = string.Empty;
         if (Settings.IncludeStackTrace) {
            stackTrace = GetStackTrace();
         }

         return new LogEntry(DateTime.Now, level, data, stackTrace);
      }

      private string GetStackTrace() {
         StringBuilder formattedStackTrace = new StringBuilder();
         var stackTrace = new StackTrace(4, true);

         return stackTrace.ToString();
      }
   }
}
