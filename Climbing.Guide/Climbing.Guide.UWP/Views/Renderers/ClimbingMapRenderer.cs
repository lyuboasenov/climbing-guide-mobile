using Climbing.Guide.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ClimbingMap), typeof(Climbing.Guide.UWP.Views.Renderers.ClimbingMapRenderer))]
namespace Climbing.Guide.UWP.Views.Renderers {
   public class ClimbingMapRenderer : MapRenderer {
      MapControl nativeMap;
      IList<Pin> Pins { get; set; }
      ClimbingMapOverlay mapOverlay;
      bool xamarinOverlayShown = false;

      protected override void OnElementChanged(ElementChangedEventArgs<Map> e) {
         base.OnElementChanged(e);

         if (e.OldElement != null) {
            nativeMap.MapElementClick -= OnMapElementClick;
            nativeMap.Children.Clear();
            mapOverlay = null;
            nativeMap = null;
         }

         if (e.NewElement != null) {
            var formsMap = (ClimbingMap)e.NewElement;
            nativeMap = Control as MapControl;
            Pins = formsMap.Pins;

            nativeMap.Children.Clear();
            nativeMap.MapElementClick += OnMapElementClick;

            foreach (var pin in Pins) {
               var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
               var snPoint = new Geopoint(snPosition);

               var mapIcon = new MapIcon();
               mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///pin.png"));
               mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
               mapIcon.Location = snPoint;
               mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);

               nativeMap.MapElements.Add(mapIcon);
            }
         }
      }

      private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args) {
         var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
         if (mapIcon != null) {
            if (!xamarinOverlayShown) {
               var pin = GetCustomPin(mapIcon.Location.Position);
               if (pin == null) {
                  throw new Exception("Custom pin not found");
               }

               if (pin.Id.ToString() == "Xamarin") {
                  if (mapOverlay == null) {
                     mapOverlay = new ClimbingMapOverlay(pin);
                  }

                  var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                  var snPoint = new Geopoint(snPosition);

                  nativeMap.Children.Add(mapOverlay);
                  MapControl.SetLocation(mapOverlay, snPoint);
                  MapControl.SetNormalizedAnchorPoint(mapOverlay, new Windows.Foundation.Point(0.5, 1.0));
                  xamarinOverlayShown = true;
               }
            } else {
               nativeMap.Children.Remove(mapOverlay);
               xamarinOverlayShown = false;
            }
         }
      }

      private Pin GetCustomPin(BasicGeoposition position) {
         var pos = new Position(position.Latitude, position.Longitude);
         foreach (var pin in Pins) {
            if (pin.Position == pos) {
               return pin;
            }
         }
         return null;
      }
   }
}
