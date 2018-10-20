using Climbing.Guide.Core.API.Schemas;

namespace Climbing.Guide.Core.Models.Routes {
   public class Grade : IGrade {
      public int AbsoluteValue { get; private set; }
      private RouteType supportedRouteType;
      public string Name { get; set; }

      public Grade(int absoluteValue, RouteType supportedRouteType, string name) {
         AbsoluteValue = absoluteValue;
         this.supportedRouteType = supportedRouteType;
         Name = name;
      }
   }
}
