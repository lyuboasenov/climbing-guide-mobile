using Climbing.Guide.Core.API.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class YosemiteGrade : Grade {
      public static readonly IGrade f3a = new YosemiteGrade(20, "5.2");
      public static readonly IGrade f3b = new YosemiteGrade(30, "5.3");
      public static readonly IGrade f3c = new YosemiteGrade(40, "5.4");
      public static readonly IGrade f4a = new YosemiteGrade(50, "5.5");
      public static readonly IGrade f4b = new YosemiteGrade(60, "5.6");
      public static readonly IGrade f5a = new YosemiteGrade(80, "5.7");
      public static readonly IGrade f5b = new YosemiteGrade(90, "5.8");
      public static readonly IGrade f5c = new YosemiteGrade(100, "5.9");
      public static readonly IGrade f6a = new YosemiteGrade(110, "5.10a");
      public static readonly IGrade f6aPlus = new YosemiteGrade(115, "5.10b");
      public static readonly IGrade f6aPlusTwo = new YosemiteGrade(117, "5.10c");
      public static readonly IGrade f6b = new YosemiteGrade(120, "5.10d");
      public static readonly IGrade f6bPlus = new YosemiteGrade(125, "5.11a");
      public static readonly IGrade f6c = new YosemiteGrade(130, "5.11b");
      public static readonly IGrade f6cPlus = new YosemiteGrade(135, "5.11c");
      public static readonly IGrade f7a = new YosemiteGrade(140, "5.11d");
      public static readonly IGrade f7aPlus = new YosemiteGrade(145, "5.12a");
      public static readonly IGrade f7b = new YosemiteGrade(150, "5.12b");
      public static readonly IGrade f7bPlus = new YosemiteGrade(155, "5.12c");
      public static readonly IGrade f7c = new YosemiteGrade(160, "5.12d");
      public static readonly IGrade f7cPlus = new YosemiteGrade(165, "5.13a");
      public static readonly IGrade f8a = new YosemiteGrade(170, "5.13b");
      public static readonly IGrade f8aPlus = new YosemiteGrade(175, "5.13c");
      public static readonly IGrade f8b = new YosemiteGrade(180, "5.13d");
      public static readonly IGrade f8bPlus = new YosemiteGrade(185, "5.14a");
      public static readonly IGrade f8c = new YosemiteGrade(190, "5.14b");
      public static readonly IGrade f8cPlus = new YosemiteGrade(195, "5.14c");
      public static readonly IGrade f9a = new YosemiteGrade(200, "5.14d");
      public static readonly IGrade f9aPlus = new YosemiteGrade(205, "5.15a");
      public static readonly IGrade f9b = new YosemiteGrade(210, "5.15b");
      public static readonly IGrade f9bPlus = new YosemiteGrade(215, "5.15c");
      public static readonly IGrade f9c = new YosemiteGrade(220, "5.15d");
      public static readonly IGrade f9cPlus = new YosemiteGrade(225, "5.16a");

      public static readonly IGrade[] Grades = new IGrade[] {
         f3a, f3b, f3c,
         f4a, f4b,
         f5a, f5b, f5c,
         f6a, f6aPlus, f6b, f6bPlus, f6c, f6cPlus, f6aPlusTwo,
         f7a, f7aPlus, f7b, f7bPlus, f7c, f7cPlus,
         f8a, f8aPlus, f8b, f8bPlus, f8c, f8cPlus,
         f9a, f9aPlus, f9b, f9bPlus, f9c, f9cPlus
      };

      private YosemiteGrade(int absoluteValue, string name) : base(absoluteValue, RouteType._2 | RouteType._4, name) {

      }
   }
}
