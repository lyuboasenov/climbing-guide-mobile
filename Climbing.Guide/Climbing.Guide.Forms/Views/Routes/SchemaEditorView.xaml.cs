using Alat.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SchemaEditorView : ContentView {
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

         // Bind actions
         UndoCommand = new Command(Undo, CanUndo);
         RedoCommand = new Command(Redo, CanRedo);

         undoButton.Command = UndoCommand;
         redoButton.Command = RedoCommand;
      }

      #region Schema Gestures
      private Rectangle _schemaPanBounds;
      private double _schemaMaxX;
      private double _schemaMaxY;
      private void PanSchema(object sender, PanUpdatedEventArgs e) {
         if (_isPanningPlaceholder)
            return;

         if (e.StatusType == GestureStatus.Started) {
            _schemaPanBounds = (Rectangle) container.GetValue(AbsoluteLayout.LayoutBoundsProperty);

            var widthToHeightRatio = schema.ImageSize.Width / schema.ImageSize.Height;

            double height = frame.Height * container.Scale;
            double width = frame.Width * container.Scale;
            if (widthToHeightRatio > 1) {
               height = width / widthToHeightRatio;
            } else {
               width = height * widthToHeightRatio;
            }

            _schemaMaxX = frame.Width - width;
            _schemaMaxY = frame.Height - height;
         } else if (e.StatusType == GestureStatus.Running) {

            // x e [maxX, 0]
            var x = Math.Min(e.TotalX + _schemaPanBounds.X, 0);
            x = Math.Max(x, _schemaMaxX);
            x = x > 0 ? 0 : x;

            // y e [maxY, 0]
            var y = Math.Min(e.TotalY + _schemaPanBounds.Y, 0);
            y = Math.Max(y, _schemaMaxY);
            y = y > 0 ? 0 : y;

            container.SetValue(AbsoluteLayout.LayoutBoundsProperty,
               new Rectangle(
                  x,
                  y,
                  _schemaPanBounds.Width,
                  _schemaPanBounds.Height));
         } else if (e.StatusType == GestureStatus.Canceled) {
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty, _schemaPanBounds);
         }

         if (e.StatusType == GestureStatus.Completed || e.StatusType == GestureStatus.Canceled) {
            _schemaPanBounds = Rectangle.Zero;
         }
      }

      private void OnDoubleTapped(object sender, EventArgs e) {
         AddPointPlaceholder();
      }

      //private Rectangle _schemaZoomBounds;
      private double _accumulatedScale = 1;
      private double _startScale = 1;
      private void ZoomContainer(object sender, PinchGestureUpdatedEventArgs e) {
         if (e.Status == GestureStatus.Started) {
            //_schemaZoomBounds = (Rectangle) schemaView.GetValue(AbsoluteLayout.LayoutBoundsProperty);

            //_childrenPanBounds = schemaViewContainer
            //   .Children.OfType<AbsoluteLayout>()
            //   .Select(c => new Tuple<View, Rectangle>(c, (Rectangle) c.GetValue(AbsoluteLayout.LayoutBoundsProperty)));
            _startScale = container.Scale;
            _accumulatedScale = _startScale;
         } else if (e.Status == GestureStatus.Running) {
            _accumulatedScale *= e.Scale;
            //schemaView.SetValue(AbsoluteLayout.LayoutBoundsProperty,
            //   new Rectangle(
            //      _schemaZoomBounds.X,
            //      _schemaZoomBounds.Y,
            //      Math.Max(_schemaZoomBounds.Width * _accumulatedScale, 1),
            //      Math.Max(_schemaZoomBounds.Height * _accumulatedScale, 1)));
            container.Scale = Math.Max(1, _accumulatedScale);

            //foreach (var child in _childrenPanBounds) {
            //   child.Item1.SetValue(AbsoluteLayout.LayoutBoundsProperty,
            //      new Rectangle(
            //         (_schemaZoomBounds.X + child.Item2.X) * _accumulatedScale,
            //         (_schemaZoomBounds.Y + child.Item2.Y) * _accumulatedScale,
            //         child.Item2.Width,
            //         child.Item2.Height));
            //}
         } else if (e.Status == GestureStatus.Canceled) {
            //schemaView.SetValue(AbsoluteLayout.LayoutBoundsProperty,
            //   _schemaZoomBounds);
            container.Scale = _startScale;
         }

         if (e.Status == GestureStatus.Completed || e.Status == GestureStatus.Canceled) {
            //_schemaZoomBounds = Rectangle.Zero;
            _startScale = 1;
         }
      }


      #endregion Schema Gestures

      private void AddPointPlaceholder() {
         var control = new AbsoluteLayout();
         control.SetValue(AbsoluteLayout.LayoutBoundsProperty,
            new Rectangle(25, 25, 10, 10));
         control.SetValue(AbsoluteLayout.LayoutFlagsProperty,
            AbsoluteLayoutFlags.None);
         control.BackgroundColor = GetRandomColor();
         var panGesture = new PanGestureRecognizer();
         panGesture.PanUpdated += PanPlaceholder;
         control.GestureRecognizers.Add(panGesture);

         container.Children.Add(control);
      }

      private Rectangle _placeholderPanLayoutBounds;
      private double _accumulatedX;
      private double _accumulatedY;
      private bool _isPanningPlaceholder = false;
      private void PanPlaceholder(object sender, PanUpdatedEventArgs e) {
         var container = (AbsoluteLayout) sender;
         if (e.StatusType == GestureStatus.Started) {
            _isPanningPlaceholder = true;
            _placeholderPanLayoutBounds = (Rectangle) container.GetValue(AbsoluteLayout.LayoutBoundsProperty);
         } else if (e.StatusType == GestureStatus.Running) {
            _accumulatedX += e.TotalX;
            _accumulatedY += e.TotalY;
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty,
               new Rectangle(
                  Math.Max(_placeholderPanLayoutBounds.X + _accumulatedX, 0),
                  Math.Max(_placeholderPanLayoutBounds.Y + _accumulatedY, 0),
                  _placeholderPanLayoutBounds.Width,
                  _placeholderPanLayoutBounds.Height));
         } else if (e.StatusType == GestureStatus.Canceled) {
            container.SetValue(AbsoluteLayout.LayoutBoundsProperty, _placeholderPanLayoutBounds);
         }

         if (e.StatusType == GestureStatus.Completed || e.StatusType == GestureStatus.Canceled) {
            _placeholderPanLayoutBounds = Rectangle.Zero;
            _isPanningPlaceholder = false;
         }

         if (e.StatusType == GestureStatus.Completed) {
            RecalculatePoints();
         }
      }

      private void RecalculatePoints() {
         Route.Clear();

         var widthToHeightRatio = schema.ImageSize.Width / schema.ImageSize.Height;

         double height = frame.Height * container.Scale;
         double width = frame.Width * container.Scale;
         if (widthToHeightRatio > 1) {
            height = width / widthToHeightRatio;
         } else {
            width = height * widthToHeightRatio;
         }

         foreach (var placeholder in container.Children.OfType<AbsoluteLayout>()) {
            var bounds = (Rectangle) placeholder.GetValue(AbsoluteLayout.LayoutBoundsProperty);

            var point = new Point(bounds.X / width, bounds.Y / height);
            Route.Add(point);
         }
      }

      private Random _random = new Random();
      private Color GetRandomColor() {
         return new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble());
      }

      private void AddPoint(Point point) {
         RouteStack.Push(point);
         UndoHistoryStack.Clear();

         RefreshSchemaRoute();
      }

      private void RefreshSchemaRoute() {
         var route = RouteStack.ToArray();
         schema.SchemaRoute = route;
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
   }
}