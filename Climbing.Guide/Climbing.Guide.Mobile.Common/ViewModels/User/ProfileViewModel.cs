using Climbing.Guide.Mobile.Common.Resources;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ProfileViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Profile_Title;

      public ProfileViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}