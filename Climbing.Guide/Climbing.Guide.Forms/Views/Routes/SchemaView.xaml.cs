using Climbing.Guide.Forms.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Routes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SchemaView : SKCanvasView {

      public static readonly BindableProperty SchemaLocalPathProperty =
         BindableProperty.Create(nameof(SchemaLocalPath), typeof(string), typeof(string), null, propertyChanged: OnSchemaLocalPathPropertyChanged);

      private static void OnSchemaLocalPathPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
         (bindable as SchemaView).InvalidateSurface();
      }

      public static readonly BindableProperty SchemaRouteProperty =
         BindableProperty.Create(nameof(SchemaRoute), typeof(IEnumerable<Point>), typeof(IEnumerable<Point>), null, propertyChanged: OnSchemaRoutePropertyChanged);

      private static void OnSchemaRoutePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
         (bindable as SchemaView).InvalidateSurface();
      }

      public string SchemaLocalPath {
         get { return (string)GetValue(SchemaLocalPathProperty); }
         set { SetValue(SchemaLocalPathProperty, value); }
      }

      public IEnumerable<Point> SchemaRoute {
         get { return (IEnumerable<Point>)GetValue(SchemaRouteProperty); }
         set { SetValue(SchemaRouteProperty, value); }
      }

      private SKPaint PathPaint { get; } = new SKPaint() { Color = SKColors.Red, StrokeWidth = 20 };

      internal Size ImageSize { get; set; }

      public SchemaView() {
         InitializeComponent();
      }

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         ImageSize = Size.Zero;

         if (!string.IsNullOrEmpty(SchemaLocalPath)) {
            using (var bitmap = SkiaSharpHelper.LoadBitmap(SchemaLocalPath, CanvasSize.Width, CanvasSize.Height))
            using (var paint = new SKPaint {
               FilterQuality = SKFilterQuality.High, // high quality scaling
               IsAntialias = true
            }) {
               ImageSize = new Size(bitmap.Width, bitmap.Height);
               e.Surface.Canvas.DrawBitmap(bitmap, 0, 0, paint);
            }
         }

         if (null != SchemaRoute) {
            SkiaSharpHelper.DrawPath(e.Surface.Canvas,
               SchemaRoute.Select(p => new Point(p.X * ImageSize.Width, p.Y * ImageSize.Height)),
               Color.Red);
         }
      }
   }
}