using System;
using System.Collections.Generic;

namespace Alat.Logging.ToLogEntryDataConverters {
   public class ExceptionToLogEntryDataConverter : ToLogEntryDataConverter {
      public LogEntryData Convert(object obj) {
         if (null == obj) {
            throw new ArgumentNullException(nameof(obj));
         }

         var exception = obj as Exception;
         if (null == exception) {
            throw new ArgumentException($"{nameof(obj)} is not Exception");
         }

         return new LogEntryData(exception.Message, CollectProperties(exception));
      }

      private IEnumerable<LogEntryProperty> CollectProperties(Exception exception) {
         var properties = new LogEntryProperty[] {
            new LogEntryProperty(nameof(exception.Message), exception.Message),
            new LogEntryProperty("Type", exception.GetType().FullName),
            new LogEntryProperty(nameof(exception.Source), exception.Source),
            new LogEntryProperty(nameof(exception.StackTrace), exception.StackTrace),
            new LogEntryProperty(nameof(exception.InnerException), CollectProperties(exception.InnerException))
         };

         return properties;
      }
   }
}
