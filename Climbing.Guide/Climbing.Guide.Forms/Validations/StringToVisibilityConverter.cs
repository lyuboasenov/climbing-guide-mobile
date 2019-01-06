using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Validations {
   public class StringToVisibilityConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         var validationErrors = value as IDictionary<string, IEnumerable<string>>;
         
         return null != validationErrors &&
            validationErrors.ContainsKey((string)parameter) &&
            validationErrors[(string)parameter].Count() > 0;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
