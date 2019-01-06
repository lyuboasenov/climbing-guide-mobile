using System;
using Climbing.Guide.Forms.Views;
using Climbing.Guide.Droid.Views.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Gms.Maps.Model;

[assembly: ExportRenderer(typeof(LocationPicker), typeof(LocationPickerRenderer))]

namespace Climbing.Guide.Droid.Views.Renderers {
   public class LocationPickerRenderer : MapRenderer {

      private Circle CenterCircle { get; set; }
      private Marker CenterMarker { get; set; }
      private LocationPicker LocationPicker { get; set; }

      public LocationPickerRenderer(Context context) : base(context) { }

      protected override void OnElementChanged(ElementChangedEventArgs<Map> e) {
         base.OnElementChanged(e);

         if (e.OldElement != null) {
            NativeMap.CameraMove -= NativeCameraMove;
            if (null != CenterCircle) {
               CenterCircle.Remove();
               CenterCircle = null;
            }

            if (null != CenterMarker) {
               CenterMarker.Remove();
               CenterMarker = null;
            }
         }

         if (e.NewElement != null) {
            LocationPicker = (LocationPicker)e.NewElement;
            Control.GetMapAsync(this);
         }
      }

      protected override void OnMapReady(Android.Gms.Maps.GoogleMap map) {
         base.OnMapReady(map);
         NativeMap.CameraMove += NativeCameraMove;
      }

      private void UpdateCircle() {
         UpdateCircle(
               LocationPicker.Location.Center.Latitude,
               LocationPicker.Location.Center.Longitude,
               LocationPicker.Location.Radius.Meters);
      }

      private void UpdateCircle(double latitude, double longitude, double radius) {

         if (LocationPicker.IsAreaVisible) {
            if (null != CenterCircle) {
               CenterCircle.Center = new LatLng(latitude, longitude);
               CenterCircle.Radius = radius;
            } else {
               var circleOptions = new CircleOptions();
               circleOptions.InvokeCenter(new LatLng(latitude, longitude));
               circleOptions.InvokeRadius(radius);
               circleOptions.InvokeFillColor(0X33FF0000);
               circleOptions.InvokeStrokeColor(0X33FF0000);
               circleOptions.InvokeStrokeWidth(0);

               CenterCircle = NativeMap.AddCircle(circleOptions);
            }
         } else if (null != CenterCircle) {
            CenterCircle.Remove();
            CenterCircle = null;
         }
      }

      private void UpdateMarker() {
         UpdateMarker(
               LocationPicker.Location.Center.Latitude,
               LocationPicker.Location.Center.Longitude);
      }

      private void UpdateMarker(double latitude, double longitude) {
         if (null != CenterMarker) {
            CenterMarker.Position = new LatLng(latitude, longitude);
         } else {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(latitude, longitude));
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));

            CenterMarker = NativeMap.AddMarker(marker);
         }
      }

      private void NativeCameraMove(object sender, EventArgs e) {
         var latitude = NativeMap.Projection.VisibleRegion.LatLngBounds.Center.Latitude;
         var longitude = NativeMap.Projection.VisibleRegion.LatLngBounds.Center.Longitude;
         var latitudeDegrees = Math.Abs(NativeMap.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               NativeMap.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);
         var longitudeDegrees = Math.Abs(NativeMap.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               NativeMap.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);

         LocationPicker.OnLocationChanged(latitude, longitude, latitudeDegrees, longitudeDegrees);

         UpdateCircle();
         UpdateMarker();
      }
   }
}