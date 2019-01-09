namespace Climbing.Guide.Forms.Services.GeoLocation.Impl {
   public class Location : Services.GeoLocation.Location {
      public double Latitude { get; }
      public double Longitude { get; }

      public Location(double latitude, double longitude) {
         Latitude = latitude;
         Longitude = longitude;
      }
   }
}
