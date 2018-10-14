namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SplashViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Main.Splash_Title;

      public SplashViewModel() {
         Title = VmTitle;
      }
   }
}