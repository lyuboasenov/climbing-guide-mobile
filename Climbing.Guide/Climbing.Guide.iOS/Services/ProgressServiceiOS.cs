using Climbing.Guide.Forms.Services;
using Climbing.Guide.iOS.Services;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFPlatform = Xamarin.Forms.Platform.iOS.Platform;

[assembly: Dependency(typeof(ProgressServiceiOS))]
namespace Climbing.Guide.iOS.Services {
   public class ProgressServiceiOS : ProgressService, IProgressService {
      private UIView LoadingDialog { get; set; }
      private UIView ProgressDialog { get; set; }

      private UIView GetDialog(Page view) {
         // build the loading page with native base
         view.Parent = Xamarin.Forms.Application.Current.MainPage;

         view.Layout(new Rectangle(0, 0,
               Xamarin.Forms.Application.Current.MainPage.Width,
               Xamarin.Forms.Application.Current.MainPage.Height));

         var renderer = view.GetOrCreateRenderer();

         return renderer.NativeView;
      }

      protected override void ShowLoadingIndicatorInternal() {
         // check if the user has set the page or not
         if (null == LoadingDialog) {
            LoadingDialog = GetDialog(LoadingView);
         }

         // showing the native loading page
         UIApplication.SharedApplication.KeyWindow.AddSubview(LoadingDialog);
      }

      protected override void HideLoadingIndicatorInternal() {
         // Hide the page
         if (null != LoadingDialog) {
            LoadingDialog.RemoveFromSuperview();
         }
      }

      protected override void ShowProgressIndicatorInternal() {
         // check if the user has set the page or not
         if (null == ProgressDialog) {
            ProgressDialog = GetDialog(ProgressView);
         }

         // showing the native loading page
         UIApplication.SharedApplication.KeyWindow.AddSubview(ProgressDialog);
      }

      protected override void HideProgressIndicatorInternal() {
         // Hide the page
         if (null != ProgressDialog) {
            ProgressDialog.RemoveFromSuperview();
         }
      }

      protected override void UpdateLoadingProgressInternal(decimal progress, decimal total, string message) {
         throw new NotImplementedException();
      }
   }

   internal static class PlatformExtension {
      public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable) {
         var renderer = XFPlatform.GetRenderer(bindable);
         if (renderer == null) {
            renderer = XFPlatform.CreateRenderer(bindable);
            XFPlatform.SetRenderer(bindable, renderer);
         }
         return renderer;
      }
   }
}