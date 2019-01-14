using System;
using System.Linq;
using Xunit;

namespace Alat.Logging.Tests {
   public class LogMessage {
      [Fact]
      public void Debug() {
         var appender = new MemorySavingAppender();
         var settings = new LoggerSettings(appender, Level.All, false);
         var logger = GetLogger(settings);


         var message = "Ping pong";
         logger.Debug(message);

         var logEntry = appender.LoggedEntries.First();

         Assert.Equal(Level.Debug, logEntry.Level);
         Assert.Equal(message, logEntry.Data.Message);
      }

      private static Logger GetLogger(LoggerSettings settings) {
         return new Impl.Logger(settings);
      }

      private static LoggerSettings GetDefaultLoggerSettings() {
         return new LoggerSettings(new MemorySavingAppender(), Level.All, true);
      }
   }
}
