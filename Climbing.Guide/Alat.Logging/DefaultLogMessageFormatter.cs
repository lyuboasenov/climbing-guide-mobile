using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Alat.Logging {
   internal class DefaultLogMessageFormatter : MessageFormatter {
      public string Format(string message, Category category, Priority priority) {
         return string.Format(
            CultureInfo.InvariantCulture, @"{{{0:u}}} {1}: {2}. Priority: {3}.", DateTime.Now,
            category.ToString().ToUpper(), message, priority);
      }
   }
}
