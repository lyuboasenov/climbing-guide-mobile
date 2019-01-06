using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Validations {
   public class ValidationErrorConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         string result = null;
         var validationErrors = value as IDictionary<string, IEnumerable<string>>;
         
         if (null != validationErrors &&
            validationErrors.ContainsKey((string)parameter)) {
            result = string.Join(Environment.NewLine, validationErrors[(string)parameter]);
         }

         return result;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
