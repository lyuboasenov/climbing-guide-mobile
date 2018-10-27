using Climbing.Guide.Mobile.Forms.Resources;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class AboutViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.About_Title;

      public AboutViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}