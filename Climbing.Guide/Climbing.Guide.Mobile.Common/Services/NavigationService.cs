using Climbing.Guide.Mobile.Common.ViewModels;
using FreshMvvm;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Services {
   internal class NavigationService : INavigationService {

      private App App { get; } = (App)App.Current;

      public void InitializeNavigation() {
         Page navigationContainer = null;
         var tasks = new[] {
            Task.Run(() => SubscribeToMessages()),
            Task.Run(() => navigationContainer = GetNavigationContainer()),
            Task.Run(() => System.Threading.Thread.Sleep(3000))
         };
         Task.WaitAll(tasks);

         // Init main navigation
         App.MainPage = navigationContainer;
      }

      public async Task PushAsync<T>(object data = null) where T : FreshBasePageModel {
         var page = FreshPageModelResolver.ResolvePageModel<T>(data);
         Views.CGMasterDetailNavigationContainer container = (Views.CGMasterDetailNavigationContainer)App.MainPage;
         await container.PushPage(page, page.GetModel(), true);
      }

      public async Task PopAsync() {
         Views.CGMasterDetailNavigationContainer container = (Views.CGMasterDetailNavigationContainer)App.MainPage;
         await container.PopPage();
      }

      public void UpdateNavigationContainer() {
         App.MainPage = GetNavigationContainer();
      }

      private Page GetNavigationContainer() {
         var userLoggedIn = ServiceLocator.Get<IRestApiClient>().IsLoggedIn;

         var masterDetailNav = new Views.CGMasterDetailNavigationContainer();
         masterDetailNav.Init(Resources.Strings.Main.CG);
         masterDetailNav.AddPage<HomeViewModel>(HomeViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.GuideViewModel>(ViewModels.Guide.GuideViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.ExploreViewModel>(ViewModels.Guide.ExploreViewModel.VmTitle);
         masterDetailNav.AddPage<ViewModels.Guide.SearchViewModel>(ViewModels.Guide.SearchViewModel.VmTitle);
         if (userLoggedIn) {
            masterDetailNav.AddPage<ViewModels.User.ProfileViewModel>(ViewModels.User.ProfileViewModel.VmTitle);
         } else {
            masterDetailNav.AddPage<ViewModels.User.LoginViewModel>(ViewModels.User.LoginViewModel.VmTitle, true, null);
         }
         masterDetailNav.AddPage<ViewModels.Settings.SettingsViewModel>(ViewModels.Settings.SettingsViewModel.VmTitle);
         masterDetailNav.AddPage<AboutViewModel>(AboutViewModel.VmTitle);
         if (userLoggedIn) {
            masterDetailNav.AddPage<ViewModels.User.LogoutViewModel>(ViewModels.User.LogoutViewModel.VmTitle);
         }
         // masterDetailNav.AddPage<ExitViewModel>(ExitViewModel.VmTitle);

         return masterDetailNav;
      }

      private void SubscribeToMessages() {
         MessagingCenter.Subscribe<Views.BaseContentPage>(App, "GoBackToMainPage", (m) => {
            Device.BeginInvokeOnMainThread(() => {
               App.MainPage = GetNavigationContainer();
            });
         });

         MessagingCenter.Subscribe<App>(App, Commands.EXIT, (m) => {
            Device.BeginInvokeOnMainThread(() => {
               System.Threading.Thread.Sleep(2000);
               App.MainPage.DisplayAlert("Ping", "Pong", "ok");
               App.Quit();
            });
         });
      }

      internal class Commands {
         public const string EXIT = "application-exit";
      }
   }
}
