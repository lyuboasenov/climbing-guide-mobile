using Climbing.Guide.Forms.Views;
using Climbing.Guide.iOS.Views.Renderers;
using CoreGraphics;
using MapKit;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ClimbingMap), typeof(ClimbingMapRenderer))]
namespace Climbing.Guide.iOS.Views.Renderers {
   public class ClimbingMapRenderer : MapRenderer {

      private readonly UITapGestureRecognizer _tapRecogniser;
      private UIView PinView { get; set; }
      private IList<Pin> Pins { get; set; }

      public ClimbingMapRenderer() {
         _tapRecogniser = new UITapGestureRecognizer(OnTap) {
            NumberOfTapsRequired = 1,
            NumberOfTouchesRequired = 1
         };
      }

      private void OnTap(UITapGestureRecognizer recognizer) {
         var cgPoint = recognizer.LocationInView(Control);

         var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);

         //Element.OnTap(new Position(location.Latitude, location.Longitude));
      }

      protected override void OnElementChanged(ElementChangedEventArgs<View> e) {
         if (Control != null)
            Control.RemoveGestureRecognizer(_tapRecogniser);

         base.OnElementChanged(e);

         if (Control != null)
            Control.AddGestureRecognizer(_tapRecogniser);

         if (e.OldElement != null) {
            var nativeMap = Control as MKMapView;
            nativeMap.GetViewForAnnotation = null;
            nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
            nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
            nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
         }

         if (e.NewElement != null) {
            var formsMap = (ClimbingMap)e.NewElement;
            var nativeMap = Control as MKMapView;
            Pins = formsMap.Pins;

            nativeMap.GetViewForAnnotation = GetViewForAnnotation;
            nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
            nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
            nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
         }
      }

      private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e) {
         var customView = e.View as CustomMKAnnotationView;
         //if (!string.IsNullOrWhiteSpace(customView.Url)) {
         //   UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
         //}
      }

      private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e) {
         var customView = e.View as CustomMKAnnotationView;
         PinView = new UIView();

         if (customView.Id == "Xamarin") {
            PinView.Frame = new CGRect(0, 0, 200, 84);
            var image = new UIImageView(new CGRect(0, 0, 200, 84));
            image.Image = UIImage.FromFile("xamarin.png");
            PinView.AddSubview(image);
            PinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
            e.View.AddSubview(PinView);
         }
      }

      private void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e) {
         if (!e.View.Selected) {
            PinView.RemoveFromSuperview();
            PinView.Dispose();
            PinView = null;
         }
      }

      private Pin GetCustomPin(MKPointAnnotation annotation) {
         var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
         foreach (var pin in Pins) {
            if (pin.Position == position) {
               return pin;
            }
         }
         return null;
      }
   }
}
