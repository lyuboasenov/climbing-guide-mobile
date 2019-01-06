using System;
using Android.Gms.Maps;
using Climbing.Guide.Forms.Views;
using Climbing.Guide.Droid.Views.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Widget;

[assembly: ExportRenderer(typeof(ClimbingMap), typeof(ClimbingMapRenderer))]

namespace Climbing.Guide.Droid.Views.Renderers {
   public class ClimbingMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter {

      private IList<Pin> Pins { get; set; }
      private ClimbingMap ClimbingMap { get; set; }

      public ClimbingMapRenderer(Context context) : base(context) { }

      public Android.Views.View GetInfoContents(Marker marker) {
         var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
         if (inflater != null) {
            var pin = GetCustomPin(marker);
            if (pin == null) {
               throw new Exception("Pin not found.");
            }

            var view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
            var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
            var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

            if (infoTitle != null) {
               infoTitle.Text = marker.Title;
            }
            if (infoSubtitle != null) {
               infoSubtitle.Text = marker.Snippet;
            }

            return view;
         }
         return null;
      }

      public Android.Views.View GetInfoWindow(Marker marker) {
         return null;
      }

      protected override void OnMapReady(GoogleMap map) {
         base.OnMapReady(map);

         NativeMap.InfoWindowClick += OnInfoWindowClick;
         NativeMap.SetInfoWindowAdapter(this);

         if (NativeMap != null) {
            NativeMap.MapLongClick += NativeMapClick;
            NativeMap.CameraMove += NativeCameraMove;
         }
      }

      protected override void OnElementChanged(ElementChangedEventArgs<Map> e) {
         base.OnElementChanged(e);

         if (e.OldElement != null) {
            NativeMap.MapLongClick -= NativeMapClick;
            NativeMap.CameraMove -= NativeCameraMove;
            NativeMap.InfoWindowClick -= OnInfoWindowClick;
         }

         if (e.NewElement != null) {
            ClimbingMap = (ClimbingMap)e.NewElement;
            Pins = ClimbingMap.Pins;
            Control.GetMapAsync(this);
         }
      }

      protected override MarkerOptions CreateMarker(Pin pin) {
         var marker = new MarkerOptions();
         marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
         marker.SetTitle(pin.Label);
         marker.SetSnippet(pin.Address);
         marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
         return marker;
      }

      private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e) {
         var customPin = GetCustomPin(e.Marker);
         if (customPin == null) {
            throw new Exception("Custom pin not found");
         }

         var climbingMap = Map as ClimbingMap;
         climbingMap.OnPinSelected(customPin.BindingContext);
      }

      private Pin GetCustomPin(Marker annotation) {
         var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
         foreach (var pin in Pins) {
            if (pin.Position == position) {
               return pin;
            }
         }
         return null;
      }

      private void NativeCameraMove(object sender, EventArgs e) {
         var latitude = NativeMap.Projection.VisibleRegion.LatLngBounds.Center.Latitude;
         var longitude = NativeMap.Projection.VisibleRegion.LatLngBounds.Center.Longitude;
         var latitudeDegrees = Math.Abs(NativeMap.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               NativeMap.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);
         var longitudeDegrees = Math.Abs(NativeMap.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude -
                               NativeMap.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);

         ClimbingMap.OnVisibleRegionChanged(latitude, longitude, latitudeDegrees, longitudeDegrees);
      }

      private void NativeMapClick(object sender, GoogleMap.MapLongClickEventArgs e) {
         ClimbingMap.OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
      }
   }
}