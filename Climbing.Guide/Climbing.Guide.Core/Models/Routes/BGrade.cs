using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class BGrade : Grade {
      public static readonly IGrade F3 = new BGrade(20, "B0");
      public static readonly IGrade F4Plus = new BGrade(35, "B1");
      public static readonly IGrade F5 = new BGrade(45, "B2");
      public static readonly IGrade F5PlusTwo = new BGrade(52, "B3");
      public static readonly IGrade F6A = new BGrade(55, "B4");
      public static readonly IGrade F6B = new BGrade(65, "B5");
      public static readonly IGrade F6CPlus = new BGrade(80, "B6");
      public static readonly IGrade F7A = new BGrade(85, "B7");
      public static readonly IGrade F7APlus = new BGrade(90, "B8");
      public static readonly IGrade F7BPlus = new BGrade(100, "B9");
      public static readonly IGrade F7C = new BGrade(105, "B10");
      public static readonly IGrade F7CPlus = new BGrade(110, "B11");
      public static readonly IGrade F8A = new BGrade(115, "B12");
      public static readonly IGrade F8APlus = new BGrade(120, "B13");
      public static readonly IGrade F8BPlus = new BGrade(130, "B14");
      public static readonly IGrade F8C = new BGrade(135, "B15");
      public static readonly IGrade F8CPlus = new BGrade(140, "B16");
      public static readonly IGrade F9A = new BGrade(145, "B17");

      public static readonly IGrade[] Grades = new IGrade[] {
         F3, F4Plus,
         F5, F5PlusTwo,
         F6A, F6B, F6CPlus,
         F7A, F7APlus, F7BPlus, F7C, F7CPlus,
         F8A, F8APlus, F8BPlus, F8C, F8CPlus,
         F9A
      };

      private BGrade(int absoluteValue, string name) : base(absoluteValue, RouteType._1, name) { }
   }
}
