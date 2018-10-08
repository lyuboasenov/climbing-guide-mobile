using Climbing.Guide.Mobile.Common.ViewModels;
using FreshMvvm;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common {
   internal class NavigationManager {
      public static Page GetNavigationContainer() {
         var masterDetailNav = new FreshMasterDetailNavigationContainer();
         masterDetailNav.Init("Menu");
         masterDetailNav.AddPage<HomeViewModel>(HomeViewModel.VmTitle);
         // masterDetailNav.AddPage<RoutesMapPageModel>("Routes");
         // masterDetailNav.AddPage<TimerTrainingListPageModel>("Timed trainings");
         // masterDetailNav.AddPage<TimerSetupPageModel>("Timer");
         masterDetailNav.AddPage<LoginViewModel>(LoginViewModel.VmTitle);

         return masterDetailNav;
      }
   }
}
