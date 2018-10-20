using Climbing.Guide.Core.API.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class UIAAGrade : Grade {
      public static readonly IGrade f3a = new UIAAGrade(20, "3-");
      public static readonly IGrade f3b = new UIAAGrade(30, "3");
      public static readonly IGrade f3c = new UIAAGrade(40, "3+");
      public static readonly IGrade f4a = new UIAAGrade(50, "4-");
      public static readonly IGrade f4b = new UIAAGrade(60, "4");
      public static readonly IGrade f4c = new UIAAGrade(70, "4+");
      public static readonly IGrade f5a = new UIAAGrade(80, "5-");
      public static readonly IGrade f5aPlus = new UIAAGrade(85, "5");
      public static readonly IGrade f5b = new UIAAGrade(90, "5+");
      public static readonly IGrade f5bPlus = new UIAAGrade(95, "5+/6-");
      public static readonly IGrade f5c = new UIAAGrade(100, "6-");
      public static readonly IGrade f5cPlus = new UIAAGrade(105, "6");
      public static readonly IGrade f6a = new UIAAGrade(110, "6+");
      public static readonly IGrade f6aPlus = new UIAAGrade(115, "6+/7-");
      public static readonly IGrade f6aPlusTwo = new UIAAGrade(117, "7-");
      public static readonly IGrade f6b = new UIAAGrade(120, "7");
      public static readonly IGrade f6bPlus = new UIAAGrade(125, "7+");
      public static readonly IGrade f6c = new UIAAGrade(130, "7+/8-");
      public static readonly IGrade f6cPlus = new UIAAGrade(135, "8-");
      public static readonly IGrade f7a = new UIAAGrade(140, "8");
      public static readonly IGrade f7aPlus = new UIAAGrade(145, "8+");
      public static readonly IGrade f7b = new UIAAGrade(150, "8+/9-");
      public static readonly IGrade f7bPlus = new UIAAGrade(155, "9-");
      public static readonly IGrade f7c = new UIAAGrade(160, "9");
      public static readonly IGrade f7cPlus = new UIAAGrade(165, "9+");
      public static readonly IGrade f8a = new UIAAGrade(170, "9+/10-");
      public static readonly IGrade f8aPlus = new UIAAGrade(175, "10-");
      public static readonly IGrade f8b = new UIAAGrade(180, "10");
      public static readonly IGrade f8bPlus = new UIAAGrade(185, "10+");
      public static readonly IGrade f8c = new UIAAGrade(190, "11-");
      public static readonly IGrade f8cPlus = new UIAAGrade(195, "11-/11");
      public static readonly IGrade f9a = new UIAAGrade(200, "11");
      public static readonly IGrade f9aPlus = new UIAAGrade(205, "11+");
      public static readonly IGrade f9b = new UIAAGrade(210, "12-");
      public static readonly IGrade f9bPlus = new UIAAGrade(215, "12");
      public static readonly IGrade f9c = new UIAAGrade(220, "12+");
      public static readonly IGrade f9cPlus = new UIAAGrade(225, "12+/13-");

      public static readonly IGrade[] Grades = new IGrade[] {
         f3a, f3b, f3c,
         f4a, f4b, f4c,
         f5a, f5aPlus, f5b, f5bPlus, f5c, f5cPlus,
         f6a, f6aPlus, f6b, f6bPlus, f6c, f6cPlus, f6aPlusTwo,
         f7a, f7aPlus, f7b, f7bPlus, f7c, f7cPlus,
         f8a, f8aPlus, f8b, f8bPlus, f8c, f8cPlus,
         f9a, f9aPlus, f9b, f9bPlus, f9c, f9cPlus
      };

      private UIAAGrade(int absoluteValue, string name) : base(absoluteValue, RouteType._2 | RouteType._4, name) {

      }
   }
}
