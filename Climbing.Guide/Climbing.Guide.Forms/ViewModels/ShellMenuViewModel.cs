using Climbing.Guide.Api;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Models;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ShellMenuViewModel : BaseViewModel, IDisposable, IAuthenticationManagerObserver {
      private IApiClient Client { get; }
      private IExceptionHandler ExceptionHandler { get; }
      private INavigation Navigation { get; }
      private IAuthenticationManager AuthenticationManager { get; }
      private IDisposable AuthenticationManagerObserverUnsubscriber { get; }

      public ObservableCollection<MenuItemModel> MenuItems { get; set; }
      public MenuItemModel SelectedMenuItem { get; set; }

      private MenuItemModel LogoutMenuItem { get; } =
         new MenuItemModel() { Title = Resources.Strings.User.Logout_Title };

      private MenuItemModel TestMenuItem { get; } = new MenuItemModel() { Title = "Test initiation" };

      public ShellMenuViewModel(IApiClient client,
         IExceptionHandler exceptionHandler,
         INavigation navigation,
         IAuthenticationManager authenticationManager) {
         Client = client;
         ExceptionHandler = exceptionHandler;
         Navigation = navigation;
         AuthenticationManager = authenticationManager;
         Title = Resources.Strings.Main.CG;

         InitializeMenuItems();

         AuthenticationManagerObserverUnsubscriber = AuthenticationManager.SubscribeObserver(this);
      }

      public async void OnSelectedMenuItemChanged() {
         await OnSelectedMenuItemChangedAsync();
      }

      public async Task OnSelectedMenuItemChangedAsync() {
         await ExceptionHandler.ExecuteErrorHandled(async () => {
            if (null != SelectedMenuItem) {
               if (SelectedMenuItem == LogoutMenuItem) {
                  await LogoutAsync();
               } else if (SelectedMenuItem == TestMenuItem) {
                  await TestAsync();
               } else {
                  await Navigation.NavigateAsync(SelectedMenuItem.NavigationRequest);
               }
            } else if (MenuItems.Count > 0) {
               SelectedMenuItem = MenuItems[0];
            }
         },
         Resources.Strings.Main.Shell_Navigation_Error_Message,
         SelectedMenuItem.Title);
      }

      public void InitializeMenuItems() {
         MenuItems = new ObservableCollection<MenuItemModel>();

         var userLoggedIn = Client.AuthenticationManager.IsLoggedIn;

         MenuItems.Add(GetMenuItem(HomeViewModel.VmTitle,
            HomeViewModel.GetNavigationRequest(Navigation)));

         MenuItems.Add(GetMenuItem(Content.List.ListGuideViewModel.VmTitle,
            Content.List.ListGuideViewModel.GetNavigationRequest(Navigation),
            Resources.Strings.Guide.Section_Title));

         MenuItems.Add(GetMenuItem(Content.List.MapGuideViewModel.VmTitle,
            Content.List.MapGuideViewModel.GetNavigationRequest(Navigation),
            Resources.Strings.Guide.Section_Title));

         if (userLoggedIn) {
            MenuItems.Add(GetMenuItem(User.ProfileViewModel.VmTitle,
               User.ProfileViewModel.GetNavigationRequest(Navigation),
               Resources.Strings.User.Section_Title));
         } else {
            MenuItems.Add(GetMenuItem(User.LoginViewModel.VmTitle,
               User.LoginViewModel.GetNavigationRequest(Navigation),
               Resources.Strings.User.Section_Title));
         }

         MenuItems.Add(GetMenuItem(Settings.SettingsViewModel.VmTitle,
            Settings.SettingsViewModel.GetNavigationRequest(Navigation),
            Resources.Strings.User.Section_Title));

         MenuItems.Add(GetMenuItem(AboutViewModel.VmTitle,
            AboutViewModel.GetNavigationRequest(Navigation)));

         if (userLoggedIn) {
            MenuItems.Add(LogoutMenuItem);
         }
#if DEBUG
         MenuItems.Add(TestMenuItem);
#endif
      }

      public void OnLogIn() {
         InitializeMenuItems();
      }

      public void OnLogOut() {
         InitializeMenuItems();
      }

      private Task TestAsync() { return Task.CompletedTask; }

      private async Task LogoutAsync() {
         await ExceptionHandler.ExecuteErrorHandled(Client.AuthenticationManager.LogoutAsync,
            Resources.Strings.Main.Communication_Error_Message,
            Resources.Strings.Main.Communication_Error_Message_Detailed_Format);

         await Navigation.NavigateAsync(HomeViewModel.GetNavigationRequest(Navigation));
      }

      private MenuItemModel GetMenuItem(string title, INavigationRequest navigationRequest, string group = null) {
         return new MenuItemModel() {
            Title = title,
            Group = group,
            NavigationRequest = Navigation.GetNavigationRequest("NavigationPage", navigationRequest)
         };
      }

      #region IDisposable Support
      private bool _disposedValue = false; // To detect redundant calls

      protected virtual void Dispose(bool disposing) {
         if (!_disposedValue) {
            if (disposing) {
               AuthenticationManagerObserverUnsubscriber.Dispose();
            }

            _disposedValue = true;
         }
      }

      ~ShellMenuViewModel() {
         // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
         Dispose(false);
      }

      // This code added to correctly implement the disposable pattern.
      public void Dispose() {
         // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      #endregion
   }
}
