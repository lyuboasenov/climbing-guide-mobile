﻿using Xamarin.Forms.Maps;

namespace Climbing.Guide.Mobile.Common.Utils {
   internal static class MapUtils {
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
