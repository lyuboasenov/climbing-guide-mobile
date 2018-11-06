using Climbing.Guide.Forms.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SchemaEditorView : ContentView {

      private const double MIN_SCALE = 1;
      private const double MAX_SCALE = 8;
      private const double OVERSHOOT = 0.15;
      private double StartX, StartY;
      private double StartScale;

      private SkiaSharp.SKPoint LastTouchLocation { get; set; }
      private Logging.ILogger LoggingService { get; set; }

      private Command UndoCommand { get; set; }
      private Command RedoCommand { get; set; }

      private Stack<Point> RouteStack { get; set; } = new Stack<Point>();
      private Stack<Point> UndoHistoryStack { get; set; } = new Stack<Point>();

      public static readonly BindableProperty RouteProperty =
         BindableProperty.Create(nameof(Route), typeof(ICollection<Point>), typeof(ICollection<Point>), null);

      public ICollection<Point> Route {
         get { return (ICollection<Point>)GetValue(RouteProperty); }
         set { SetValue(RouteProperty, value); }
      }

      public SchemaEditorView() {
         InitializeComponent();
         LoggingService = IoC.Container.Get<Logging.ILogger>();

         // Attach gestures
         schemaView.EnableTouchEvents = true;
         schemaView.Touch += OnTouch;

         var tapGesture = new TapGestureRecognizer {
            NumberOfTapsRequired = 2
         };
         tapGesture.Tapped += OnDoubleTapped;
         schemaView.GestureRecognizers.Add(tapGesture);

         var pinchGesture = new PinchGestureRecognizer();
         pinchGesture.PinchUpdated += OnPinchUpdated;
         schemaView.GestureRecognizers.Add(pinchGesture);

         var panGesture = new PanGestureRecognizer();
         panGesture.PanUpdated += OnPanUpdated;
         schemaView.GestureRecognizers.Add(panGesture);

         // Bind actions
         UndoCommand = new Command(Undo, CanUndo);
         RedoCommand = new Command(Redo, CanRedo);

         undoButton.Command = UndoCommand;
         redoButton.Command = RedoCommand;
      }

      private void OnPanUpdated(object sender, PanUpdatedEventArgs e) {
         switch (e.StatusType) {
            case GestureStatus.Started:
               StartX = (1 - schemaView.AnchorX) * Width;
               StartY = (1 - schemaView.AnchorY) * Height;
               break;

            case GestureStatus.Running:
               schemaView.AnchorX = (1 - (StartX + e.TotalX) / Width).Clamp(0, 1);
               schemaView.AnchorY = (1 - (StartY + e.TotalY) / Height).Clamp(0, 1);
               break;
         }
      }

      private void OnDoubleTapped(object sender, EventArgs e) {
         var point = LastTouchLocation.ToPoint();
         point.X = Math.Min(point.X / schemaView.ImageSize.Width, 1);
         point.Y = Math.Min(point.Y / schemaView.ImageSize.Height, 1);

         //AddPointPlaceholder(LastTouchLocation.ToPoint());
         AddPoint(point);
      }

      private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) {
         LoggingService.Log("Schema editor pinch.");
         switch (e.Status) {
            case GestureStatus.Started:
               StartScale = schemaView.Scale;
               schemaView.AnchorX = e.ScaleOrigin.X;
               schemaView.AnchorY = e.ScaleOrigin.Y;
               break;

            case GestureStatus.Running:
               double current = schemaView.Scale + (e.Scale - 1) * StartScale;
               schemaView.Scale = current.Clamp(MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
               break;

            case GestureStatus.Completed:
               if (schemaView.Scale > MAX_SCALE)
                  schemaView.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
               else if (Scale < MIN_SCALE)
                  schemaView.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
               break;
         }
      }

      private void OnTouch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e) {
         LastTouchLocation = e.Location;
      }

      private void AddPoint(Point point) {
         RouteStack.Push(point);
         UndoHistoryStack.Clear();

         RefreshSchemaRoute();
      }

      //private void AddPointPlaceholder(Point point) {
      //   var placeholder = GetPointPlaceholder();
      //   schemaViewContainer.Children.Add(placeholder, point);
      //}

      private void RefreshSchemaRoute() {
         var route = RouteStack.ToArray();
         schemaView.SchemaRoute = route;
         if (null != Route) {
            Route.Clear();
            foreach(var point in route) {
               Route.Add(point);
            }
         }

         UndoCommand.ChangeCanExecute();
         RedoCommand.ChangeCanExecute();
      }

      private bool CanUndo() {
         return RouteStack.Count() > 0;
      }

      private void Undo() {
         UndoHistoryStack.Push(RouteStack.Pop());

         RefreshSchemaRoute();
      }

      private bool CanRedo() {
         return UndoHistoryStack.Count > 0;
      }

      private void Redo() {
         RouteStack.Push(UndoHistoryStack.Pop());

         RefreshSchemaRoute();
      }

      //private View GetPointPlaceholder() {
      //   var panGesture = new PanGestureRecognizer();
      //   panGesture.PanUpdated += OnPointPlaceholderPanUpdate;

      //   Label placeholder = new Label() {
      //      FontFamily = Forms.Resources.FontFileResources.FontAwesomeSolid,
      //      Text = Forms.Resources.GlyphNames.ArrowsAlt
      //   };
      //   placeholder.GestureRecognizers.Add(panGesture);

      //   return placeholder;
      //}

      //private void OnPointPlaceholderPanUpdate(object sender, PanUpdatedEventArgs e) {
      //   throw new NotImplementedException();
      //}
   }
}