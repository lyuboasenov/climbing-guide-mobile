using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Settings {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SettingsViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Settings.Settings_Title;

      public SettingsViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}