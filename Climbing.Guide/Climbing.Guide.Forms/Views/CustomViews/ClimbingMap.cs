using Climbing.Guide.Forms.Helpers;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.Views {
   public class ClimbingMap : Map {

      private MapSpan currentVisibleRegion;
      private Pin selectedLocationPin;

      public static readonly BindableProperty PinTappedProperty =
         BindableProperty.Create(nameof(PinTapped), typeof(ICommand), typeof(ClimbingMap), null);

      public static readonly BindableProperty ShowSelectedLocationProperty =
         BindableProperty.Create(nameof(ShowSelectedLocation), typeof(bool), typeof(ClimbingMap), true);

      public static readonly BindableProperty SelectedLocationProperty =
         BindableProperty.Create(nameof(SelectedLocation), typeof(Position), typeof(ClimbingMap), new Position(0, 0),
            propertyChanged: OnSelectedLocationChanged);

      public static readonly BindableProperty PinsSourceProperty =
         BindableProperty.Create(nameof(PinsSource), typeof(IEnumerable), typeof(ClimbingMap), null,
            propertyChanged: PinsSourceChanged);

      public static readonly BindableProperty BindableVisibleRegionProperty =
         BindableProperty.Create(nameof(BindableVisibleRegion), typeof(MapSpan), typeof(ClimbingMap), null,
            propertyChanged: OnBindableVisibleRegionChanged);

      public ICommand PinTapped {
         get { return (ICommand)GetValue(PinTappedProperty); }
         set { SetValue(PinTappedProperty, value); }
      }

      public bool ShowSelectedLocation {
         get { return (bool)GetValue(ShowSelectedLocationProperty); }
         set { SetValue(ShowSelectedLocationProperty, value); }
      }

      public Position SelectedLocation {
         get { return (Position)GetValue(SelectedLocationProperty); }
         set { SetValue(SelectedLocationProperty, value); }
      }

      public IEnumerable PinsSource {
         get { return (IEnumerable)GetValue(PinsSourceProperty); }
         set { SetValue(PinsSourceProperty, value); }
      }

      private BindingBase pinLabelBinding;
      public BindingBase PinLabelBinding {
         get { return pinLabelBinding; }
         set {
            if (pinLabelBinding == value)
               return;

            OnPropertyChanging();
            var oldValue = value;
            pinLabelBinding = value;
            OnPinLabelDisplayBindingChanged(oldValue, pinLabelBinding);
            OnPropertyChanged();
         }
      }

      private BindingBase pinPositionBinding;
      public BindingBase PinPositionBinding {
         get { return pinPositionBinding; }
         set {
            if (pinPositionBinding == value)
               return;

            OnPropertyChanging();
            var oldValue = value;
            pinPositionBinding = value;
            OnPinLabelDisplayBindingChanged(oldValue, pinPositionBinding);
            OnPropertyChanged();
         }
      }

      private BindingBase pinTypeBinding;
      public BindingBase PinTypeBinding {
         get { return pinTypeBinding; }
         set {
            if (pinTypeBinding == value)
               return;

            OnPropertyChanging();
            var oldValue = value;
            pinTypeBinding = value;
            OnPinLabelDisplayBindingChanged(oldValue, pinTypeBinding);
            OnPropertyChanged();
         }
      }

      private BindingBase pinAddressBinding;
      public BindingBase PinAddressBinding {
         get { return pinAddressBinding; }
         set {
            if (pinAddressBinding == value)
               return;

            OnPropertyChanging();
            var oldValue = value;
            pinAddressBinding = value;
            OnPinLabelDisplayBindingChanged(oldValue, pinAddressBinding);
            OnPropertyChanged();
         }
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
         ((ClimbingMap)bindable).OnSelectedLocationChanged();
      }

      private static void OnBindableVisibleRegionChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((ClimbingMap)bindable).OnBindableVisibleRegionChanged();
      }

      private void OnSelectedLocationChanged() {
         if (ShowSelectedLocation) {
            if (null == selectedLocationPin) {
               selectedLocationPin = new Pin() {Label = Climbing.Guide.Forms.Resources.Strings.Main.Current_Location };
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

      private static void PinsSourceChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((ClimbingMap)bindable).ResetPins();
      }

      private void OnPinLabelDisplayBindingChanged(BindingBase oldValue, BindingBase newValue) {
         ResetPins();
      }

      private void ResetPins() {
         Pins.Clear();
         if (null != PinsSource) {
            foreach (var pinData in PinsSource) {
               var pin = new Pin() { BindingContext = pinData };

               if (null != PinLabelBinding) { pin.SetBinding(Pin.LabelProperty, PinLabelBinding.Clone()); }
               else { pin.Label = pinData.ToString(); }

               if (null != PinPositionBinding) { pin.SetBinding(Pin.PositionProperty, PinPositionBinding.Clone()); }
               if (null != PinTypeBinding) { pin.SetBinding(Pin.TypeProperty, PinTypeBinding.Clone()); }
               if (null != PinAddressBinding) { pin.SetBinding(Pin.TypeProperty, PinAddressBinding.Clone()); }

               Pins.Add(pin);
            }
         }
         //As all the pins are removed we call on selected loation changed 
         //so the selected location can be reinitialized
         OnSelectedLocationChanged();
      }

      public void OnPinSelected(object data) {
         if (null != PinTapped && PinTapped.CanExecute(data)) {
            PinTapped.Execute(data);
         }
      }
   }
}
