using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Converters
{
   public class LongToBytesConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         long bytes = (long)value;
         string result = $"{bytes} B";
         if (parameter.Equals("KB")) {
            result = $"{bytes / Math.Pow(2, 10):0.00} KB";
         } else if (parameter.Equals("MB")) {
            result = $"{bytes / Math.Pow(2, 20):0.00} MB";
         } else if (parameter.Equals("GB")) {
            result = $"{bytes / Math.Pow(2, 40):0.00} MB";
         } else if (parameter.Equals("TB")) {
            result = $"{bytes / Math.Pow(2, 80):0.00} MB";
         }
         return result;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
