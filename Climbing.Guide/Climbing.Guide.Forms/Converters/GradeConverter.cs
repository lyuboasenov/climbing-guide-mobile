﻿using Climbing.Guide.Api.Schemas;
using System;
using System.Globalization;
using Xamarin.Forms;
using Climbing.Guide.Forms.Services;
using System.Linq;
using System.Threading.Tasks;
using Climbing.Guide.Tasks;

namespace Climbing.Guide.Forms.Converters {
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
            var prefService = Services.IoC.Container.Get<IPreferences>();
            var routeType = route.Type;
            int gradingSystemId = 1;

            if (routeType == RouteType._1) { gradingSystemId = prefService.BoulderingGradeSystem; }
            else if (routeType == RouteType._2) { gradingSystemId = prefService.SportRouteGradeSystem; }
            else if (routeType == RouteType._4) { gradingSystemId = prefService.TradRouteGradeSystem; }
            Grade grade = null;

            var taskRunner = Services.IoC.Container.Get<ISyncTaskRunner>();
            grade = taskRunner.RunSync(() => Services.IoC.Container.Get<IResource>().GetGradeSystemAsync(gradingSystemId))
               .Where(g => g.Value <= route.Difficulty).OrderByDescending(g => g.Value).First();

            if (null != grade) {
               result = grade.Name;
            }
         }
         return result;
      }
   }
}
