namespace Climbing.Guide.Forms.Services.GeoLocation {
   public class Location : ILocation {
      public double Latitude { get; }
      public double Longitude { get; }

      public Location(double latitude, double longitude) {
         Latitude = latitude;
         Longitude = longitude;
      }
   }
}
