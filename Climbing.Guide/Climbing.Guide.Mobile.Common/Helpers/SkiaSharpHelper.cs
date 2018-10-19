using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Helpers {
   public static class SkiaSharpHelper {

      public static SKColor ToSkColor(this Color color) {
         return new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
      }

      public static SKPaint GetPaint(SKColor color) {
         return new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
         };
      }

      public static SKBitmap HandleOrientation(this SKBitmap bitmap, SKCodecOrigin orientation) {
         switch (orientation) {
            case SKCodecOrigin.BottomRight:

               using (var surface = new SKCanvas(bitmap)) {
                  surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                  surface.DrawBitmap(bitmap.Copy(), 0, 0);
               }

               break;

            case SKCodecOrigin.RightTop:
               var workingCopy = new SKBitmap(bitmap.Height, bitmap.Width);

               using (var surface = new SKCanvas(workingCopy)) {
                  surface.Translate(workingCopy.Width, 0);
                  surface.RotateDegrees(90);
                  surface.DrawBitmap(bitmap, 0, 0);
               }

               bitmap.Dispose();
               bitmap = workingCopy;
               break;

            case SKCodecOrigin.LeftBottom:
               var workingCopy2 = new SKBitmap(bitmap.Height, bitmap.Width);

               using (var surface = new SKCanvas(workingCopy2)) {
                  surface.Translate(0, workingCopy2.Height);
                  surface.RotateDegrees(270);
                  surface.DrawBitmap(bitmap, 0, 0);
               }

               bitmap.Dispose();
               bitmap = workingCopy2;
               break;
         }

         return bitmap;
      }

      public static SKBitmap LoadBitmap(string location, double maxSize) {
         using (var input = File.OpenRead(location))                   // load the file
         using (var inputStream = new SKManagedStream(input))          // create a sream SkiaSharp uses
         using (var codec = SKCodec.Create(inputStream)) {             // get the decoder

            var bitmap = SKBitmap.Decode(codec).HandleOrientation(codec.Origin);

            // Determine scaling factor
            var bitmapAspectRatio = (double)bitmap.Width / bitmap.Height;
            double factor = bitmapAspectRatio > 1 ? maxSize / bitmap.Width : maxSize / bitmap.Height;

            SKImageInfo info = new SKImageInfo((int)(bitmap.Width * factor), (int)(bitmap.Height * factor));
            bitmap = bitmap.Resize(info, SKBitmapResizeMethod.Triangle);

            return bitmap;
         }
      }

      //public static SKBitmap Export(this RouteTemplate route) {
      //   var bitmap = LoadBitmap(route.ImageLocation, RouteTemplate.MAX_SIZE);
      //   SKCanvas canvas = new SKCanvas(bitmap);

      //   //DrawHolds
      //   foreach (var hold in route.Holds) {
      //      DrawHold(canvas, hold);
      //   }

      //   //DrawPaths
      //   foreach (Path path in route.Paths) {
      //      DrawPath(canvas, path);
      //   }

      //   return bitmap;
      //}

      public static void DrawPath(SKCanvas canvas, IEnumerable<Point> path, Color color, Point? originPoint = null, double scaleFactor = 1) {
         canvas.DrawPath(path.ConvertToSKPath(originPoint, scaleFactor), GetPaint(color.ToSkColor()));
      }

      //public static void DrawHold(SKCanvas canvas, Hold hold, Point? originPoint = null, double scaleFactor = 1) {
      //   double offsetX = originPoint?.X ?? 0;
      //   double offsetY = originPoint?.Y ?? 0;

      //   canvas.DrawCircle(
      //      (float)((hold.Center.X - offsetX) * scaleFactor),
      //      (float)((hold.Center.Y - offsetY) * scaleFactor),
      //      (float)(hold.Radius * scaleFactor),
      //      GetPaint(hold.Color.ToSkColor()));
      //}

      /// <summary>
      /// Calculates distance between two skpoints.
      /// </summary>
      /// <param name="p1"></param>
      /// <param name="p2"></param>
      /// <returns></returns>
      public static double Distance(this SKPoint p1, SKPoint p2) {
         return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
      }

      /// <summary>
      /// Converts Path to SKPath
      /// </summary>
      /// <param name="path">Path to be converted</param>
      /// <param name="originPoint">Origin point if transposition is to be made</param>
      /// <param name="scaleFactor">Scale factor if scaling is to be made</param>
      /// <returns></returns>
      public static SKPath ConvertToSKPath(this IEnumerable<Point> path, Point? originPoint = null, double scaleFactor = 1) {
         var normalizedPath = new SKPath();
         var convertedPath = path.Select(p => p.ConvertToSKPoint(originPoint, scaleFactor)).ToArray();
         if (convertedPath.Length > 0) {
            normalizedPath.MoveTo(convertedPath[0]);

            if (convertedPath.Length >= 3) {
               for (int i = 2; i < convertedPath.Length; i++) {
                  var point1 = convertedPath[i - 2];
                  var point2 = convertedPath[i - 1];
                  var point3 = convertedPath[i];
                  normalizedPath.CubicTo(point1, point2, point3);
               }
            } else {
               foreach (var point in convertedPath) {
                  normalizedPath.LineTo(point);
               }
            }
         }

         return normalizedPath;
      }

      /// <summary>
      /// Converts a Point to SKPoint
      /// </summary>
      /// <param name="point">Point to be converted</param>
      /// <param name="originPoint">Origin point if transposition is to be made</param>
      /// <param name="scaleFactor">Scale factor if scaling is to be made</param>
      /// <returns></returns>
      public static SKPoint ConvertToSKPoint(this Point point, Point? originPoint = null, double scaleFactor = 1) {
         double originX = originPoint?.X ?? 0;
         double originY = originPoint?.Y ?? 0;
         return new SKPoint((float)((point.X - originX) * scaleFactor), (float)((point.Y - originY) * scaleFactor));
      }
   }
}
