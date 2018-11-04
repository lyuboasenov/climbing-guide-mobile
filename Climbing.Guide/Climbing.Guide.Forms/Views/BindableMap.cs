using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.Views {
   public class BindableMap : Map {

      private MapSpan currentVisibleRegion;

      public static readonly BindableProperty ShowSelectedLocationProperty =
         BindableProperty.Create(nameof(ShowSelectedLocation), typeof(bool), typeof(BindableMap), true);

      public static readonly BindableProperty SelectedLocationProperty =
         BindableProperty.Create(nameof(SelectedLocation), typeof(Position), typeof(BindableMap), new Position(0, 0),
            propertyChanged: OnSelectedLocationChanged);

      public static readonly BindableProperty BindablePinsProperty =
         BindableProperty.Create(nameof(BindablePins), typeof(ObservableCollection<Pin>), typeof(BindableMap), null,
            propertyChanged: OnBindablePinsChanged);

      public static readonly BindableProperty BindableVisibleRegionProperty =
         BindableProperty.Create(nameof(BindableVisibleRegion), typeof(MapSpan), typeof(BindableMap), null,
            propertyChanged: OnBindableVisibleRegionChanged);

      public BindableMap() {
         
      }

      public bool ShowSelectedLocation {
         get { return (bool)GetValue(ShowSelectedLocationProperty); }
         set { SetValue(ShowSelectedLocationProperty, value); }
      }

      public Position SelectedLocation {
         get { return (Position)GetValue(SelectedLocationProperty); }
         set { SetValue(SelectedLocationProperty, value); }
      }

      public ObservableCollection<Pin> BindablePins {
         get { return (ObservableCollection<Pin>)GetValue(BindablePinsProperty); }
         set { SetValue(BindablePinsProperty, value); }
      }

      public MapSpan BindableVisibleRegion {
         get { return (MapSpan)GetValue(BindableVisibleRegionProperty); }
         set { SetValue(BindableVisibleRegionProperty, value); }
      }

      public void OnTap(Position position) {
         SelectedLocation = position;
      }
      
      public void OnVisibleRegionChanged(double latitude, double longitude, double latitudeDegrees, double longitudeDegrees) {
         var center = new Position(latitude, longitude);
         //CurrentVisibleRegion is saved in order not to move to it
         //on property changed. This is done because MapSpan is been calculated
         //on camera move which causes some data lost. which makes the maps to 
         //move constantly
         currentVisibleRegion = new MapSpan(center, latitudeDegrees, longitudeDegrees);
         BindableVisibleRegion = currentVisibleRegion;
      }

      private static void OnSelectedLocationChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((BindableMap)bindable).OnSelectedLocationChanged();
      }

      private static void OnBindableVisibleRegionChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((BindableMap)bindable).OnBindableVisibleRegionChanged();
      }

      private Pin selectedLocationPin;

      private void OnSelectedLocationChanged() {
         if (ShowSelectedLocation) {
            if (null == selectedLocationPin) {
               selectedLocationPin = new Pin() {Label = Forms.Resources.Strings.Main.Current_Location };
            }
            selectedLocationPin.Position = SelectedLocation;
            if (!Pins.Contains(selectedLocationPin)) {
               Pins.Add(selectedLocationPin);
            }
         }
      }

      private void OnBindableVisibleRegionChanged() {
         //the maps is moved only if the action is initiated from outside
         //not from the control itself
         if (BindableVisibleRegion != currentVisibleRegion) {
            this.MoveToRegion(BindableVisibleRegion);
            currentVisibleRegion = BindableVisibleRegion;
         }
      }

      private static void OnBindablePinsChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((BindableMap)bindable).OnBindablePinsChanged();
      }

      private void OnBindablePinsChanged() {
         Pins.Clear();
         if (null != BindablePins) {
            foreach (var pin in BindablePins) {
               Pins.Add(pin);
            }
         }
         //As all the pins are removed we call on selected loation changed 
         //so the selected location can be reinitialized
         OnSelectedLocationChanged();
      }
   }
}
