using Climbing.Guide.Forms.Views;
using Climbing.Guide.iOS.Views.Renderers;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LocationPicker), typeof(LocationPickerRenderer))]
namespace Climbing.Guide.iOS.Views.Renderers {
   public class LocationPickerRenderer : MapRenderer {

      private LocationPicker LocationPicker { get; set; }

      public LocationPickerRenderer() {

      }

      protected override void OnElementChanged(ElementChangedEventArgs<View> e) {
         
         base.OnElementChanged(e);
         
         if (e.OldElement != null) {
            var nativeMap = Control as MKMapView;
            nativeMap.RegionChanged -= OnRegionChanged;
         }

         if (e.NewElement != null) {
            LocationPicker = (LocationPicker)e.NewElement;
            var nativeMap = Control as MKMapView;

            nativeMap.RegionChanged += OnRegionChanged;
         }
      }

      private void OnRegionChanged(object sender, MKMapViewChangeEventArgs e) {
         if(null != LocationPicker) {
            var currentRegion = ((MKMapView)Control).Region;
            LocationPicker.OnLocationPropertyChanged(
               currentRegion.Center.Latitude,
               currentRegion.Center.Longitude,
               currentRegion.Span.LatitudeDelta,
               currentRegion.Span.LongitudeDelta);
         }
      }
   }
}
