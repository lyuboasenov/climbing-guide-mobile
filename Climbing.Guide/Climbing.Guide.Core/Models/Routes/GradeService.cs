using Climbing.Guide.Core.API.Schemas;
using System.Collections.Generic;
using System.Linq;

namespace Climbing.Guide.Core.Models.Routes {
   public class GradeService : IGradeService {

      private Dictionary<GradeType, IEnumerable<IGrade>> Grades { get; set; } =
         new Dictionary<GradeType, IEnumerable<IGrade>>() {
            { GradeType.FontBolder, FontBoulderGrade.Grades },
            { GradeType.V, VGrade.Grades },
            { GradeType.B, BGrade.Grades },
            { GradeType.FrenchRoute, FrenchRouteGrade.Grades },
            { GradeType.YosemiteGrade, YosemiteGrade.Grades },
            { GradeType.UIAA, UIAAGrade.Grades },
         };

      public IGrade GetGrade(int absoluteValue, GradeType gradeType) {
         // Filters grades selecting only ones with absolute value equals or less to supplied one.
         // Orders them by their absolute value in discending order and select the first one.
         return GetGradeList(gradeType).Where(g => g.AbsoluteValue <= absoluteValue).OrderByDescending(g => g.AbsoluteValue).First();
      }

      public IEnumerable<IGrade> GetGradeList(GradeType gradeType) {
         return Grades[gradeType];
      }

      ///// <summary>
      /////     BOULDER = 1
      /////     SPORT = 2
      /////     TRAD = 4
      ///// </summary>
      ///// <param name="routeType"></param>
      ///// <returns></returns>
      //public static IEnumerable<Grade> GetGradeList(RouteType routeType) {
      //   IEnumerable<Grade> result = null;
      //   switch (routeType) {
      //      case RouteType._1:
      //         result = FontBoulderGrade.Grades;
      //         break;
      //      case RouteType._2:
      //      case RouteType._4:
      //         result = FrenchRouteGrade.Grades;
      //         break;
      //   }
      //   return result;
      //}
   }
}
