using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Mobile.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SchemaEditorView : ContentView {
      private Logging.ILoggingService LoggingService { get; set; }
      public SchemaEditorView() {
         InitializeComponent();
         LoggingService = IoC.Container.Get<Logging.ILoggingService>();

         var tapGesture = new TapGestureRecognizer {
            NumberOfTapsRequired = 2
         };
         tapGesture.Tapped += OnDoubleTapped;

         schemaView.GestureRecognizers.Add(tapGesture);

         var pinchGesture = new PinchGestureRecognizer();
         pinchGesture.PinchUpdated += OnPinchUpdated;

         schemaView.GestureRecognizers.Add(pinchGesture);
      }

      private void OnDoubleTapped(object sender, EventArgs e) {
         LoggingService.Log("Schema editor tap.");
      }

      double currentScale = 1;
      double startScale = 1;
      double xOffset = 0;
      double yOffset = 0;

      private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) {
         LoggingService.Log("Schema editor pinch.");
         if (e.Status == GestureStatus.Started) {
            // Store the current scale factor applied to the wrapped user interface element,
            // and zero the components for the center point of the translate transform.
            startScale = schemaView.Scale;
            schemaView.AnchorX = 0;
            schemaView.AnchorY = 0;
         }
         if (e.Status == GestureStatus.Running) {
            // Calculate the scale factor to be applied.
            currentScale += (e.Scale - 1) * startScale;
            currentScale = Math.Max(1, currentScale);

            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the X pixel coordinate.
            double renderedX = schemaView.X + xOffset;
            double deltaX = renderedX / Width;
            double deltaWidth = Width / (schemaView.Width * startScale);
            double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the Y pixel coordinate.
            double renderedY = schemaView.Y + yOffset;
            double deltaY = renderedY / Height;
            double deltaHeight = Height / (schemaView.Height * startScale);
            double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

            // Calculate the transformed element pixel coordinates.
            double targetX = xOffset - (originX * schemaView.Width) * (currentScale - startScale);
            double targetY = yOffset - (originY * schemaView.Height) * (currentScale - startScale);

            // Apply translation based on the change in origin.
            schemaView.TranslationX = targetX.Clamp(-schemaView.Width * (currentScale - 1), 0);
            schemaView.TranslationY = targetY.Clamp(-schemaView.Height * (currentScale - 1), 0);

            // Apply scale factor.
            schemaView.Scale = currentScale;
         }
         if (e.Status == GestureStatus.Completed) {
            // Store the translation delta's of the wrapped user interface element.
            xOffset = schemaView.TranslationX;
            yOffset = schemaView.TranslationY;
         }
      }

      private void OnTouch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e) {
         LoggingService.Log($"Schema editor touch ({e.Location.X}, {e.Location.Y}).");
      }
   }
}