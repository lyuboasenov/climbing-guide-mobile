using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class HomeViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.Home_Title;
      public static NavigationRequest GetNavigationRequest(Navigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.HomeView));
      }

      public HomeViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }


}