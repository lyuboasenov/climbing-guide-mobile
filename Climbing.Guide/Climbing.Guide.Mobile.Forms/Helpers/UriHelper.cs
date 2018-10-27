using System;

namespace Climbing.Guide.Mobile.Forms.Helpers {
   public static class UriHelper {
      public enum Schema {
         nav,
         act,
         type
      }

      public static Uri Get(Schema schema, string path) {
         return new Uri($"{schema}://forms.climbingguide.org/{path}");
      }

      public static Uri GetChild(this Uri parent, string path) {
         return new Uri(parent, $"{parent.LocalPath}/{path}");
      }

      public static Uri GetTypeUri(Type type, int number) {
         return (new UriBuilder($"{Schema.type}", type.FullName, number)).Uri;
      }
   }
}
