using Climbing.Guide.Api.Schemas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.Converters {
   public class ToPositionConverter : IValueConverter {
      private Guide.Api.Schemas.Region region;
      private Area area;
      private Sector sector;
      private Route route;

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         decimal latitude = -1;
         decimal longitude = -1;

         if (null != (region = value as Guide.Api.Schemas.Region)) {
            latitude = region.Latitude;
            longitude = region.Longitude;
         } else if (null != (area = value as Area)) {
            latitude = area.Latitude;
            longitude = area.Longitude;
         } else if (null != (sector = value as Sector)) {
            latitude = sector.Latitude;
            longitude = sector.Longitude;
         } else if (null != (route = value as Route)) {
            latitude = route.Latitude;
            longitude = route.Longitude;
         }

         if (latitude == -1 || longitude == -1) {
            return null;
         } else {
            return new Position((double)latitude, (double)longitude);
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
