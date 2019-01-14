using Alat.Logging.LogEntryFormatters;
using System;

namespace Alat.Logging.Appenders {
   public class VoidAppender : LoggerAppender {
      public LogEntryFormatter LogEntryFormatter { get; }

      public VoidAppender(LogEntryFormatter logEntryFormatter) {
         LogEntryFormatter = logEntryFormatter ?? throw new ArgumentNullException(nameof(logEntryFormatter));
      }

      public void Write(LogEntry entry) {

      }
   }
}
