using Climbing.Guide.Forms.Views;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(LocationPicker), typeof(Climbing.Guide.UWP.Views.Renderers.LocationPickerRenderer))]
namespace Climbing.Guide.UWP.Views.Renderers {
   public class LocationPickerRenderer : MapRenderer {
      private MapControl NativeMap { get; set; }
      private LocationPicker LocationPicker { get; set; }

      protected override void OnElementChanged(ElementChangedEventArgs<Map> e) {
         base.OnElementChanged(e);

         if (e.OldElement != null) {
            NativeMap.Children.Clear();
            NativeMap.ActualCameraChanged -= NativeMap_ActualCameraChanged;
         }

         if (e.NewElement != null) {
            LocationPicker = (LocationPicker)e.NewElement;
            NativeMap = Control as MapControl;

            NativeMap.Children.Clear();
            NativeMap.ActualCameraChanged += NativeMap_ActualCameraChanged;
         }
      }

      private void NativeMap_ActualCameraChanged(MapControl sender, MapActualCameraChangedEventArgs args) {
         var visibleRegion = sender.GetVisibleRegion(MapVisibleRegionKind.Full);
         // TODO: Change visible region
         //LocationPicker.OnLocationPropertyChanged(
         //   sender.Center.Position.Latitude,
         //   sender.Center.Position.Longitude, 
         //   sender.ActualCamera. 
         //   );
      }
   }
}
