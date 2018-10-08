using Climbing.Guide.Mobile.Common.Resources;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;

      public SignupViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}