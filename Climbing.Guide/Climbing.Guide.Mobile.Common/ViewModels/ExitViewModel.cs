using Climbing.Guide.Mobile.Common.Resources;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExitViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.Exit_Title;

      public ExitViewModel() {
         Title = VmTitle;
         MessagingCenter.Send<App>((App)App.Current, NavigationManager.Commands.EXIT);
      }
   }
}