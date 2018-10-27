using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using Climbing.Guide.Mobile.Forms.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XFPlatform = Xamarin.Forms.Platform.Android.Platform;
using Plugin.CurrentActivity;

[assembly: Dependency(typeof(Climbing.Guide.Mobile.Droid.Services.ProgressServiceDroid))]
namespace Climbing.Guide.Mobile.Droid.Services {
   public class ProgressServiceDroid : ProgressService, IProgressService {
      private Dialog ProgressDialog { get; set; }
      private Dialog LoadingDialog { get; set; }

      private Dialog GetDialog(Page view) {
         // build the loading page with native base
         view.Parent = Xamarin.Forms.Application.Current.MainPage;

         view.Layout(new Rectangle(0, 0,
               Xamarin.Forms.Application.Current.MainPage.Width,
               Xamarin.Forms.Application.Current.MainPage.Height));

         var renderer = view.GetOrCreateRenderer();
            
         var dialog = new Dialog(CrossCurrentActivity.Current.Activity);
         dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
         dialog.SetCancelable(false);
         dialog.SetContentView(renderer.View);
         Window window = dialog.Window;
         window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
         window.ClearFlags(WindowManagerFlags.DimBehind);
         window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

         return dialog;
      }

      private void XamFormsPage_Appearing(object sender, EventArgs e) {
         var animation = new Animation(callback: d => ((ContentPage)sender).Content.Rotation = d,
                                       start: ((ContentPage)sender).Content.Rotation,
                                       end: ((ContentPage)sender).Content.Rotation + 360,
                                       easing: Easing.Linear);
         animation.Commit(((ContentPage)sender).Content, "RotationLoopAnimation", 16, 800, null, null, () => true);
      }

      protected override void HideLoadingIndicatorInternal() {
         // Hide the page
         // check if the user has set the page or not
         if (null != LoadingDialog) {
            LoadingDialog.Hide();
         }
      }

      protected override void ShowLoadingIndicatorInternal() {
         // check if the user has set the page or not
         if (null == LoadingDialog) {
            LoadingDialog = GetDialog(LoadingView);
         }
         // showing the native loading page
         LoadingDialog.Show();
      }

      protected override void ShowProgressIndicatorInternal() {
         // check if the user has set the page or not
         if (null == ProgressDialog) {
            ProgressDialog = GetDialog(ProgressView);
         }
         // showing the native loading page
         ProgressDialog.Show();
      }

      protected override void HideProgressIndicatorInternal() {
         // Hide the page
         // check if the user has set the page or not
         if (null != ProgressDialog) {
            ProgressDialog.Hide();
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
            renderer = XFPlatform.CreateRendererWithContext(bindable, CrossCurrentActivity.Current.Activity);
            XFPlatform.SetRenderer(bindable, renderer);
         }
         return renderer;
      }
   }
}