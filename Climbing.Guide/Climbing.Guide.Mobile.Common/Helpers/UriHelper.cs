using System;

namespace Climbing.Guide.Mobile.Common.Helpers {
   public static class UriHelper {
      public enum Schema {
         nav,
         act
      }

      public static Uri Get(Schema schema, string path) {
         return new Uri($"{schema}://forms.climbingguide.org/{path}");
      }

      public static Uri GetChild(this Uri parent, string path) {
         return new Uri(parent, $"{parent.LocalPath}/{path}");
      }
   }
}
