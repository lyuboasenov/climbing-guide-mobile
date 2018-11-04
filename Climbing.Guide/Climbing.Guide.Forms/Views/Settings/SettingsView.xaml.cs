using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.Settings {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SettingsView : TabbedPage {
      public SettingsView() {
         InitializeComponent();
      }
   }
}