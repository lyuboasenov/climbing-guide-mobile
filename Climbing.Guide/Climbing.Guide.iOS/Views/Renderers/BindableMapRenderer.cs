using Climbing.Guide.Forms.Views;
using Climbing.Guide.iOS.Views.Renderers;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BindableMap), typeof(BindableMapRenderer))]
namespace Climbing.Guide.iOS.Views.Renderers {
   public class BindableMapRenderer : MapRenderer {
      private readonly UITapGestureRecognizer _tapRecogniser;

      public BindableMapRenderer() {
         _tapRecogniser = new UITapGestureRecognizer(OnTap) {
            NumberOfTapsRequired = 1,
            NumberOfTouchesRequired = 1
         };
      }

      private void OnTap(UITapGestureRecognizer recognizer) {
         var cgPoint = recognizer.LocationInView(Control);

         var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);

         Element.OnTap(new Position(location.Latitude, location.Longitude));
      }

      protected override void OnElementChanged(ElementChangedEventArgs<View> e) {
         if (Control != null)
            Control.RemoveGestureRecognizer(_tapRecogniser);

         base.OnElementChanged(e);

         if (Control != null)
            Control.AddGestureRecognizer(_tapRecogniser);
      }
   }
}
