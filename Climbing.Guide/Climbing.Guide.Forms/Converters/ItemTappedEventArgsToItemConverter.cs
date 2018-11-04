using System;
using System.Globalization;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Converters {
   public class ItemTappedEventArgsToItemConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         var eventArgs = value as ItemTappedEventArgs;
         return eventArgs.Item;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
