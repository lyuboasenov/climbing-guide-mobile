using System.Collections.Generic;
using Alat.Logging.LogEntryFormatters;

namespace Alat.Logging.Tests {
   public class MemorySavingAppender : Appenders.LoggerAppender {
      private IList<LogEntry> loggedEntries = new List<LogEntry>();

      public LogEntryFormatter LogEntryFormatter { get; }
      public IEnumerable<LogEntry> LoggedEntries { get { return loggedEntries; } }

      public void Write(LogEntry entry) {
         loggedEntries.Add(entry);
      }
   }
}
