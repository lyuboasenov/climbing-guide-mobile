using Climbing.Guide.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.GeoLocation {
   public class GeoLocation : IGeoLocation {
      private const int secondsTimeout = 5;
      private readonly Location defaultLocation = new Location(42.6735598, 23.3626882);

      public async Task<ILocation> GetCurrentOrDefaultAsync(CancellationToken token = default(CancellationToken)) {
         Location result = null;
         try {
            result = await GetCurrentAsync(token);
         } catch {
            try {
               result = await GetLastKnownAsync();
            } catch {
               result = defaultLocation;
            }
         }

         return result;
      }

      private static async Task<Location> GetCurrentAsync(CancellationToken token = default(CancellationToken)) {
         var result = await Xamarin.Essentials.Geolocation.GetLocationAsync(new Xamarin.Essentials.GeolocationRequest(), token).
               TimeoutAfter(TimeSpan.FromSeconds(secondsTimeout));

         return Convert(result);
      }

      private static async Task<Location> GetLastKnownAsync() {
         var result = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync().
               TimeoutAfter(TimeSpan.FromSeconds(secondsTimeout));

         return Convert(result);
      }

      private static Location Convert(Xamarin.Essentials.Location location) {
         return new Location(location.Latitude, location.Longitude);
      }
   }
}
