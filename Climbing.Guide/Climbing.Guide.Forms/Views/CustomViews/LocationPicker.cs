using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.Views {
   public class LocationPicker : Map {
      public static readonly BindableProperty LocationProperty =
         BindableProperty.Create(nameof(Location), typeof(MapSpan), typeof(LocationPicker), null,
            propertyChanged: (b, o, n) => { ((LocationPicker)b).OnLocationChanged(); });

      public bool IsAreaVisible { get; set; } = true;

      public MapSpan Location {
         get { return (MapSpan)GetValue(LocationProperty)??VisibleRegion; }
         set { SetValue(LocationProperty, value); }
      }

      private MapSpan ControlInitiatedLocation { get; set; }

      public void OnLocationChanged(double latitude, double longitude, double latitudeDegrees, double longitudeDegrees) {
         // CurrentVisibleRegion is saved in order not to move to it
         // on property changed. This is done because MapSpan is been calculated
         // on camera move which causes some data lost. which makes the maps to 
         // move constantly
         ControlInitiatedLocation = new MapSpan(new Position(latitude, longitude), latitudeDegrees, longitudeDegrees);
         Location = ControlInitiatedLocation;
      }

      private void OnLocationChanged() {
         // the maps is moved only if the action is initiated from outside
         // not from the control itself
         if (Location != ControlInitiatedLocation) {
            MoveToRegion(Location);
            ControlInitiatedLocation = Location;
         }
      }
   }
}
