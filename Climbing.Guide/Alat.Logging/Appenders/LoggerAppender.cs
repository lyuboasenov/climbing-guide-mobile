using Alat.Logging.LogEntryFormatters;

namespace Alat.Logging.Appenders {
   public interface LoggerAppender {
      LogEntryFormatter LogEntryFormatter { get; }
      void Write(LogEntry entry);
   }
}