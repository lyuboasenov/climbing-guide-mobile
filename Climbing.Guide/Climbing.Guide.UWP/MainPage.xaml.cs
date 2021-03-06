﻿using Xamarin.Forms.Platform.UWP;

namespace Climbing.Guide.UWP {
   /// <summary>
   /// An empty page that can be used on its own or navigated to within a Frame.
   /// </summary>
   public sealed partial class MainPage : WindowsPage {
      public MainPage() {
         this.InitializeComponent();
         Rg.Plugins.Popup.Popup.Init();
         Xamarin.FormsMaps.Init("INSERT_MAP_KEY_HERE");
         this.LoadApplication(new Forms.App());
      }
   }
}
