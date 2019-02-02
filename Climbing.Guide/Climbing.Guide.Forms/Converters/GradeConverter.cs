using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Commands;
using Climbing.Guide.Forms.Services.IoC;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Converters {
   public class GradeConverter : IValueConverter {
      private RouteGradeQuery RouteGradeQuery { get; }

      public GradeConverter() : this(Container.Get<ICommandQueryFactory>()) { }

      internal GradeConverter(ICommandQueryFactory commandQueryFactory) {
         if (commandQueryFactory == null)
            throw new ArgumentNullException(nameof(commandQueryFactory));

         RouteGradeQuery = commandQueryFactory.GetQuery<RouteGradeQuery>();
      }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         return Convert(value as Route);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
         throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
      }

      public string Convert(Route route) {
         string result = string.Empty;
         if (route != null) {
            RouteGradeQuery.Route = route;

            try {
               result = RouteGradeQuery.GetResult().Name;
            } catch (KeyNotFoundException) {
               result = string.Empty;
            }
         }

         return result;
      }
   }
}
