using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Mobile.Common.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ExitView : BaseContentPage {
      public ExitView() {
         InitializeComponent();
         App.Current.Quit();
      }
   }
}