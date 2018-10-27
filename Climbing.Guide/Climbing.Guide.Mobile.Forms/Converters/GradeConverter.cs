using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Core.Models.Routes;
using System;
using System.Globalization;
using Xamarin.Forms;
using Climbing.Guide.Mobile.Forms.Services;

namespace Climbing.Guide.Mobile.Forms.Converters {
   public class GradeConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         return Convert(value as Route);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }

      public static string Convert(Route route) {
         string result = string.Empty;
         if (null != route) {
            result = IoC.Container.Get<IGradeService>().GetGrade(route.Difficulty, GradeType.V).Name;
         }
         return result;
      }
   }
}
