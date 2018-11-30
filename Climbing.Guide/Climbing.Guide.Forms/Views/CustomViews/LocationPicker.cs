using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.Views {
   public class LocationPicker : Map {

      private MapSpan CurrentLocation { get; set; }
      public bool IsAreaVisible { get; set; } = true;

      public static readonly BindableProperty LocationProperty =
         BindableProperty.Create(nameof(Location), typeof(MapSpan), typeof(LocationPicker), null,
            propertyChanged: OnLocationChanged);

      public MapSpan Location {
         get { return (MapSpan)GetValue(LocationProperty)??VisibleRegion; }
         set { SetValue(LocationProperty, value); }
      }
      
      public void OnLocationChanged(double latitude, double longitude, double latitudeDegrees, double longitudeDegrees) {
         var center = new Position(latitude, longitude);
         //CurrentVisibleRegion is saved in order not to move to it
         //on property changed. This is done because MapSpan is been calculated
         //on camera move which causes some data lost. which makes the maps to 
         //move constantly
         CurrentLocation = new MapSpan(center, latitudeDegrees, longitudeDegrees);
         Location = CurrentLocation;
      }

      private static void OnLocationChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((LocationPicker)bindable).OnLocationChanged();
      }

      private void OnLocationChanged() {
         //the maps is moved only if the action is initiated from outside
         //not from the control itself
         if (Location != CurrentLocation) {
            this.MoveToRegion(Location);
            CurrentLocation = Location;
         }
      }
   }
}
