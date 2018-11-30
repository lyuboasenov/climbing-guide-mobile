using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Climbing.Guide.UWP {
   public sealed partial class ClimbingMapOverlay : UserControl {
      private Pin Pin { get; set; }
      public ClimbingMapOverlay(Pin pin) {
         this.InitializeComponent();
         Pin = pin;
         SetupData();
      }

      private void SetupData() {
         Label.Text = Pin.Label;
         Address.Text = Pin.Address;
      }

      private async void OnInfoButtonTapped(object sender, TappedRoutedEventArgs e) {
         //await Launcher.LaunchUriAsync(new Uri(Pin));
      }
   }
}
