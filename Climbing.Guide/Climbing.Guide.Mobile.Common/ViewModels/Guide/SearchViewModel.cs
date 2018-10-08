﻿using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SearchViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Search_Title;

      public SearchViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}