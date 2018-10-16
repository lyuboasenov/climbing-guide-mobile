using Climbing.Guide.Core.API;
using Climbing.Guide.Mobile.Common.ViewModels;
using FreshMvvm;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common {
   internal class NavigationManager {

      public static NavigationManager Current { get; } = new NavigationManager();

      private App App { get; } = (App)App.Current;

      public async Task InitializeNavigationAsync() {
         MessagingCenter.Subscribe<Views.BaseContentPage>(Current.App, "GoBackToMainPage", (m) => {
            Device.BeginInvokeOnMainThread(() => {
               Current.App.MainPage = GetNavigationContainerAsync();
            });
         });

         MessagingCenter.Subscribe<App>(Current, Commands.EXIT, (m) => {
            Device.BeginInvokeOnMainThread(() => {
               System.Threading.Thread.Sleep(2000);
               //var closer = DependencyService.Get<Services.ICloseApplication>();
               //closer?.closeApplication();
               Current.App.MainPage.DisplayAlert("Ping", "Pong", "ok");
               Current.App.Quit();
            });
         });

         Page navigationContainer = null;
         var tasks = new[] {
            Task.Run(() => navigationContainer = Current.GetNavigationContainerAsync()),
            Task.Run(() => System.Threading.Thread.Sleep(5000))
         };
         Task.WaitAll(tasks);

         // Init main navigation
         Current.App.MainPage = navigationContainer;
      }

      public async Task PushModalAsync(Page page) {
         Views.CGMasterDetailNavigationContainer container = (Views.CGMasterDetailNavigationContainer)App.MainPage;
         await container.Navigation.PushModalAsync(page);
      }

      public async Task PushModalAsync<T>(object data = null) where T : FreshBasePageModel {
         var page = FreshPageModelResolver.ResolvePageModel<T>(data);
         Views.CGMasterDetailNavigationContainer container = (Views.CGMasterDetailNavigationContainer)App.MainPage;
         //await container.Navigation.PushAsync(page);
         await container.PushPage(page, page.GetModel(), true);
      }

      public async Task PopModalAsync() {
         Views.CGMasterDetailNavigationContainer container = (Views.CGMasterDetailNavigationContainer)App.MainPage;
         //await container.Navigation.PopAsync();
         await container.PopPage();
      }

      private Page GetNavigationContainerAsync() {
         var masterDetailNav = new Views.CGMasterDetailNavigationContainer();
         masterDetailNav.Init(Resources.Strings.Main.CG);
         masterDetailNav.AddPage<HomeViewModel>(HomeViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.GuideViewModel>(ViewModels.Guide.GuideViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.ExploreViewModel>(ViewModels.Guide.ExploreViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.SearchViewModel>(ViewModels.Guide.SearchViewModel.VmTitle);
         if (RestApiClient.Instance.IsLoggedIn) {
            masterDetailNav.AddPage<ViewModels.User.ProfileViewModel>(ViewModels.User.ProfileViewModel.VmTitle);
         } else {
            masterDetailNav.AddPage<ViewModels.User.LoginViewModel>(ViewModels.User.LoginViewModel.VmTitle, true, null);
         }
         masterDetailNav.AddPage<ViewModels.Settings.SettingsViewModel>(ViewModels.Settings.SettingsViewModel.VmTitle);
         masterDetailNav.AddPage<AboutViewModel>(AboutViewModel.VmTitle);
         if (RestApiClient.Instance.IsLoggedIn) {
            masterDetailNav.AddPage<ViewModels.User.LogoutViewModel>(ViewModels.User.LogoutViewModel.VmTitle);
         }
         // masterDetailNav.AddPage<ExitViewModel>(ExitViewModel.VmTitle);

         return masterDetailNav;
      }

      public void UpdateNavigationContainerAsync() {
         Current.App.MainPage = Current.GetNavigationContainerAsync();
      }

      internal class Commands {
         public const string EXIT = "application-exit";
      }
   }
}
