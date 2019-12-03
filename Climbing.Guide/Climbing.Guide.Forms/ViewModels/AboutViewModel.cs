using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class AboutViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.About_Title;
      public static INavigationRequest GetNavigationRequest(Services.Navigation.INavigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.AboutView));
      }

      public AboutViewModel() {
         Title = VmTitle;

         OpenWebCommand = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
      }

      public ICommand OpenWebCommand { get; }
   }
}