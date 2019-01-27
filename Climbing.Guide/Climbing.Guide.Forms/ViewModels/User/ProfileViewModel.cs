using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ProfileViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Profile_Title;
      public static INavigationRequest GetNavigationRequest(Services.Navigation.INavigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.User.ProfileView));
      }

      public ProfileViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}