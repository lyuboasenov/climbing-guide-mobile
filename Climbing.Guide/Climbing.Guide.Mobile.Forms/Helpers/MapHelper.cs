using Xamarin.Forms.Maps;

namespace Climbing.Guide.Mobile.Forms.Helpers {
   internal static class MapHelper {
      public static Position GetPosition(decimal latitude, decimal longitude) {
         return new Position((double)latitude, (double)longitude);
      }

      public static Pin GetPin(string name, decimal latitude, decimal longitude, PinType type = PinType.Generic, object data = null) {
         return new Pin() {
            Type = type,
            Label = name,
            Position = new Position((double)latitude, (double)longitude),
            BindingContext = data
         };
      }
   }
}
