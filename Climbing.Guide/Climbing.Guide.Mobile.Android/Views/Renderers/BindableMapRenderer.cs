using System;
using Android.Gms.Maps;
using Climbing.Guide.Mobile.Common.Views;
using Climbing.Guide.Mobile.Droid.Views.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BindableMap), typeof(BindableMapRenderer))]

namespace Climbing.Guide.Mobile.Droid.Views.Renderers {
   public class BindableMapRenderer : MapRenderer {
      // We use a native google map for Android
      private GoogleMap _map;

      protected override void OnMapReady(GoogleMap map) {
         base.OnMapReady(map);
         _map = map;

         if (_map != null) {
            _map.MapLongClick += googleMap_MapClick;
            //_map.CameraChange += googleMap_CameraChange;
            _map.CameraMove += googleMap_CameraMove;

         }
      }

      protected override void OnElementChanged(ElementChangedEventArgs<Map> e) {
         if (_map != null) {
            _map.MapLongClick -= googleMap_MapClick;
            //_map.CameraChange -= googleMap_CameraChange;
            _map.CameraMove += googleMap_CameraMove;
         }

         base.OnElementChanged(e);

         if (Control != null) {
            ((MapView)Control).GetMapAsync(this);
         }
      }

      private void googleMap_CameraMove(object sender, EventArgs e) {
         var latitude = _map.Projection.VisibleRegion.LatLngBounds.Center.Latitude;
         var longitude = _map.Projection.VisibleRegion.LatLngBounds.Center.Longitude;
         var latitudeDegrees = Math.Abs(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               _map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);
         var longitudeDegrees = Math.Abs(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               _map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);

         ((BindableMap)Element).OnVisibleRegionChanged(latitude, longitude, latitudeDegrees, longitudeDegrees);
      }

      private void googleMap_MapClick(object sender, GoogleMap.MapLongClickEventArgs e) {
         ((BindableMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
      }
   }
}