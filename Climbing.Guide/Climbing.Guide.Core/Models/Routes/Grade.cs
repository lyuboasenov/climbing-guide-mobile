using Climbing.Guide.Core.API.Schemas;
using System.Collections.Generic;

namespace Climbing.Guide.Core.Models.Routes {
   public class Grade {
      public int AbsoluteValue { get; private set; }
      private RouteType supportedRouteType;
      public string Name { get; set; }

      public Grade(int absoluteValue, RouteType supportedRouteType, string name) {
         AbsoluteValue = absoluteValue;
         this.supportedRouteType = supportedRouteType;
         Name = name;
      }
      /// <summary>
      ///     BOULDER = 1
      ///     SPORT = 2
      ///     TRAD = 4
      /// </summary>
      /// <param name="routeType"></param>
      /// <returns></returns>
      public static IEnumerable<Grade> GetGradeList(RouteType routeType) {
         IEnumerable<Grade> result = null;
         switch (routeType) {
            case RouteType._1:
               result = FontBoulderGrade.Grades;
               break;
            case RouteType._2:
            case RouteType._4:
               result = FrenchRouteGrade.Grades;
               break;
         }
         return result;
      }
   }
}
