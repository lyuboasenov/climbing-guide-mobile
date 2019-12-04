using Alat.Logging;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SchemaDevelopmentView : ContentPage {
      private readonly Random _random = new Random();

      public SchemaDevelopmentView() {
         InitializeComponent();
      }

      private void OnTapGestureRecognizerTapped(object sender, EventArgs args) {
         var container = (AbsoluteLayout) sender;

         var control = new AbsoluteLayout();
         control.SetValue(AbsoluteLayout.LayoutBoundsProperty,
            new Rectangle(25, 25, 10, 10));
         control.SetValue(AbsoluteLayout.LayoutFlagsProperty,
            AbsoluteLayoutFlags.None);
         control.BackgroundColor = GetRandomColor();
         var panGesture = new PanGestureRecognizer();
         panGesture.PanUpdated += OnPanUpdated;
         control.GestureRecognizers.Add(panGesture);

         container.Children.Add(control);
      }

      private Color GetRandomColor() {
         return new Color(_random.NextDouble());
      }

      private Rectangle _containerStartLayoutBounds;
      private double _accumulatedScale = 1;
      private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) {
         var container = (AbsoluteLayout) sender;
         if (e.Status == GestureStatus.Started) {
            _containerStartLayoutBounds = (Rectangle) container.GetValue(AbsoluteLayout.LayoutBoundsProperty);
         } else if (e.Status == GestureStatus.Running) {
            _accumulatedScale *= e.Scale;
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty,
               new Rectangle(
                  _containerStartLayoutBounds.X,
                  _containerStartLayoutBounds.Y,
                  Math.Max(_containerStartLayoutBounds.Width * _accumulatedScale, 50),
                  Math.Max(_containerStartLayoutBounds.Height * _accumulatedScale, 50)));
         } else if (e.Status == GestureStatus.Canceled) {
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty,
               _containerStartLayoutBounds);
         }

         if (e.Status == GestureStatus.Completed || e.Status == GestureStatus.Canceled) {
            _containerStartLayoutBounds = Rectangle.Zero;
            _accumulatedScale = 1;
         }
      }

      private double _accumulatedX;
      private double _accumulatedY;
      private void OnPanUpdated(object sender, PanUpdatedEventArgs e) {
         var container = (AbsoluteLayout) sender;
         if (e.StatusType == GestureStatus.Started) {
            _containerStartLayoutBounds = (Rectangle) container.GetValue(AbsoluteLayout.LayoutBoundsProperty);
         } else if (e.StatusType == GestureStatus.Running) {
            _accumulatedX += e.TotalX;
            _accumulatedY += e.TotalY;
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty,
               new Rectangle(
                  Math.Max(_containerStartLayoutBounds.X + _accumulatedX, 0),
                  Math.Max(_containerStartLayoutBounds.Y + _accumulatedY, 0),
                  _containerStartLayoutBounds.Width,
                  _containerStartLayoutBounds.Height));
         } else if (e.StatusType == GestureStatus.Canceled) {
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty, _containerStartLayoutBounds);
         }

         if (e.StatusType == GestureStatus.Completed || e.StatusType == GestureStatus.Canceled) {
            _containerStartLayoutBounds = Rectangle.Zero;
         }
      }
   }
}