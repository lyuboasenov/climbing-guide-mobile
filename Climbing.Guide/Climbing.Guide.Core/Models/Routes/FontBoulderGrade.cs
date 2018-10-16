using Climbing.Guide.Core.API.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class FontBoulderGrade : Grade {
      public static readonly FontBoulderGrade F3 = new FontBoulderGrade(20, "3");
      public static readonly FontBoulderGrade F4Minus = new FontBoulderGrade(25, "4-");
      public static readonly FontBoulderGrade F4 = new FontBoulderGrade(30, "4");
      public static readonly FontBoulderGrade F4Plus = new FontBoulderGrade(35, "4+");
      public static readonly FontBoulderGrade F5Minus = new FontBoulderGrade(40, "5-");
      public static readonly FontBoulderGrade F5 = new FontBoulderGrade(45, "5");
      public static readonly FontBoulderGrade F5Plus = new FontBoulderGrade(50, "5+");
      public static readonly FontBoulderGrade F6A = new FontBoulderGrade(55, "6A");
      public static readonly FontBoulderGrade F6APlus = new FontBoulderGrade(60, "6A+");
      public static readonly FontBoulderGrade F6B = new FontBoulderGrade(65, "6B");
      public static readonly FontBoulderGrade F6BPlus = new FontBoulderGrade(70, "6B+");
      public static readonly FontBoulderGrade F6C = new FontBoulderGrade(75, "6C");
      public static readonly FontBoulderGrade F6CPlus = new FontBoulderGrade(80, "6C+");
      public static readonly FontBoulderGrade F7A = new FontBoulderGrade(85, "7A");
      public static readonly FontBoulderGrade F7APlus = new FontBoulderGrade(90, "7A+");
      public static readonly FontBoulderGrade F7B = new FontBoulderGrade(95, "7B");
      public static readonly FontBoulderGrade F7BPlus = new FontBoulderGrade(100, "7B+");
      public static readonly FontBoulderGrade F7C = new FontBoulderGrade(105, "7C");
      public static readonly FontBoulderGrade F7CPlus = new FontBoulderGrade(110, "7C+");
      public static readonly FontBoulderGrade F8A = new FontBoulderGrade(115, "8A");
      public static readonly FontBoulderGrade F8APlus = new FontBoulderGrade(120, "8A+");
      public static readonly FontBoulderGrade F8B = new FontBoulderGrade(125, "8B");
      public static readonly FontBoulderGrade F8BPlus = new FontBoulderGrade(130, "8B+");
      public static readonly FontBoulderGrade F8C = new FontBoulderGrade(135, "8C");
      public static readonly FontBoulderGrade F8CPlus = new FontBoulderGrade(140, "8C+");
      public static readonly FontBoulderGrade F9A = new FontBoulderGrade(145, "9A");

      public static readonly Grade[] Grades = new Grade[] {
         F3, F4Minus, F4, F4Plus,
         F5Minus, F5, F5Plus,
         F6A, F6APlus, F6B, F6BPlus, F6C, F6CPlus,
         F7A, F7APlus, F7B, F7BPlus, F7C, F7CPlus,
         F8A, F8APlus, F8B, F8BPlus, F8C, F8CPlus,
         F9A
      };

      private FontBoulderGrade(int absoluteValue, string name) : base(absoluteValue, RouteType._1, name) { }
   }
}
