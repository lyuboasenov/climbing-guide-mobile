using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class VGrade : Grade {
      public static readonly IGrade F3 = new VGrade(20, "VB");
      public static readonly IGrade F4Minus = new VGrade(25, "V0-");
      public static readonly IGrade F4Plus = new VGrade(35, "V0");
      public static readonly IGrade F5 = new VGrade(45, "V0+");
      public static readonly IGrade F5PlusTwo = new VGrade(52, "V1");
      public static readonly IGrade F6A = new VGrade(55, "V2");
      public static readonly IGrade F6APlus = new VGrade(60, "V3");
      public static readonly IGrade F6B = new VGrade(65, "V3+");
      public static readonly IGrade F6BPlus = new VGrade(70, "V4");
      public static readonly IGrade F6C = new VGrade(75, "V4+");
      public static readonly IGrade F6CPlus = new VGrade(80, "V5");
      public static readonly IGrade F7A = new VGrade(85, "V6");
      public static readonly IGrade F7APlus = new VGrade(90, "V7");
      public static readonly IGrade F7BPlus = new VGrade(100, "V8");
      public static readonly IGrade F7C = new VGrade(105, "V9");
      public static readonly IGrade F7CPlus = new VGrade(110, "V10");
      public static readonly IGrade F8A = new VGrade(115, "V11");
      public static readonly IGrade F8APlus = new VGrade(120, "V12");
      public static readonly IGrade F8B = new VGrade(125, "V13");
      public static readonly IGrade F8BPlus = new VGrade(130, "V14");
      public static readonly IGrade F8C = new VGrade(135, "V15");
      public static readonly IGrade F8CPlus = new VGrade(140, "V16");
      public static readonly IGrade F9A = new VGrade(145, "V17");

      public static readonly IGrade[] Grades = new IGrade[] {
         F3, F4Minus, F4Plus,
         F5, F5PlusTwo,
         F6A, F6APlus, F6B, F6BPlus, F6C, F6CPlus,
         F7A, F7APlus, F7BPlus, F7C, F7CPlus,
         F8A, F8APlus, F8B, F8BPlus, F8C, F8CPlus,
         F9A
      };

      private VGrade(int absoluteValue, string name) : base(absoluteValue, RouteType._1, name) { }
   }
}
