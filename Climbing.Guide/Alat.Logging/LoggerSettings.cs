using Alat.Logging.Appenders;
using Alat.Logging.ToLogEntryDataConverters;
using System;
using System.Collections.Generic;

namespace Alat.Logging {
   public class LoggerSettings {
      public IEnumerable<LoggerAppender> LoggerAppenders { get; }
      public IEnumerable<KeyValuePair<Type, ToLogEntryDataConverter>> ToLogEntryDataConverters { get; }
      public Level Level { get; }
      public bool IncludeStackTrace { get; }

      public LoggerSettings(IEnumerable<LoggerAppender> loggerAppenders,
         IEnumerable<KeyValuePair<Type, ToLogEntryDataConverter>> toLogEntryDataConverters,
         Level level,
         bool includeStackTrace) {
         LoggerAppenders = loggerAppenders ?? throw new ArgumentNullException(nameof(loggerAppenders));
         ToLogEntryDataConverters = toLogEntryDataConverters ?? throw new ArgumentNullException(nameof(toLogEntryDataConverters));
         Level = level ?? throw new ArgumentNullException(nameof(level));
         IncludeStackTrace = includeStackTrace;
      }

      public LoggerSettings(LoggerAppender loggerAppender,
         IEnumerable<KeyValuePair<Type, ToLogEntryDataConverter>> toLogEntryDataConverters,
         Level level,
         bool includeStackTrace) : this(new LoggerAppender[] { loggerAppender },
            Array.Empty<KeyValuePair<Type, ToLogEntryDataConverter>>(),
            level,
            includeStackTrace
            ) { }

      public LoggerSettings(IEnumerable<LoggerAppender> loggerAppenders,
         Level level,
         bool includeStackTrace) : this(loggerAppenders,
            Array.Empty<KeyValuePair<Type, ToLogEntryDataConverter>>(),
            level,
            includeStackTrace
            ) { }

      public LoggerSettings(LoggerAppender loggerAppender,
         Level level,
         bool includeStackTrace) : this(new LoggerAppender[] { loggerAppender },
            level,
            includeStackTrace
            ) { }
   }
}
