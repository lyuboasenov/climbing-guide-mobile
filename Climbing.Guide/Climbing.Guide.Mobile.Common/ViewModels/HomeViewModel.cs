﻿using Climbing.Guide.Mobile.Common.Resources;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class HomeViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.Home_Title;

      public HomeViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}