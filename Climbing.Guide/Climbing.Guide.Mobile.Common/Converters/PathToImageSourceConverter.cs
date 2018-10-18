using System;
using System.Globalization;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Converters {
   public class PathToImageSourceConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         return ImageSource.FromFile(value as string);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
