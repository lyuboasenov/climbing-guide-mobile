using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Common.Helpers;
using Climbing.Guide.Mobile.Common.Models;
using Climbing.Guide.Mobile.Common.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ShellMenuViewModel : BaseViewModel {
      public ObservableCollection<MenuItemModel> MenuItems { get; set; }
      public MenuItemModel SelectedMenuItem { get; set; }

      private Uri LogoutUri { get; } = UriHelper.Get(UriHelper.Schema.act, "Logout");

      public ShellMenuViewModel() : base() {
         Title = Resources.Strings.Main.CG;
         InitializeMenuItems();

         GetService<IEventService>().GetEvent<Events.ShellMenuInalidated>().Subscribe((payload) => {
            InitializeMenuItems();
         });
      }

      public void OnSelectedMenuItemChanged() {
         Task.Run(async () => await OnSelectedMenuItemChangedAsync());
      }

      public async Task OnSelectedMenuItemChangedAsync() {
         try {
            if (null != SelectedMenuItem) {
               if (SelectedMenuItem.NavigationUri == LogoutUri) {
                  await LogoutAsync();
               } else {
                  await GetService<INavigationService>().NavigateAsync(SelectedMenuItem.NavigationUri);
               }
            } else if (MenuItems.Count > 0) {
               SelectedMenuItem = MenuItems[0];
            }
         }catch (Exception ex) {
            GetService<IErrorService>().LogException(ex);
         }
      }

      public void InitializeMenuItems() {
         MenuItems = new ObservableCollection<MenuItemModel>();

         var userLoggedIn = IoC.Container.Get<IRestApiClient>().IsLoggedIn;

         MenuItems.Add(GetMenuItem(HomeViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.HomeView))));

         MenuItems.Add(GetMenuItem(Guide.GuideViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.Guide.GuideView)),
            Resources.Strings.Guide.Section_Title));

         MenuItems.Add(GetMenuItem(Guide.ExploreViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.Guide.ExploreView)),
            Resources.Strings.Guide.Section_Title));

         MenuItems.Add(GetMenuItem(Guide.SearchViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.Guide.SearchView)),
            Resources.Strings.Guide.Section_Title));

         if (userLoggedIn) {
            MenuItems.Add(GetMenuItem(User.ProfileViewModel.VmTitle,
               NavigationService.GetShellNavigationUri(nameof(Views.User.ProfileView)),
               Resources.Strings.User.Section_Title));
         } else {
            MenuItems.Add(GetMenuItem(User.LoginViewModel.VmTitle,
               NavigationService.GetShellNavigationUri(nameof(Views.User.LoginView)),
               Resources.Strings.User.Section_Title));
         }

         MenuItems.Add(GetMenuItem(Settings.SettingsViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.Settings.SettingsView)),
            Resources.Strings.User.Section_Title));

         MenuItems.Add(GetMenuItem(AboutViewModel.VmTitle,
            NavigationService.GetShellNavigationUri(nameof(Views.AboutView))));

         if (userLoggedIn) {
            MenuItems.Add(GetMenuItem(Resources.Strings.User.Logout_Title,
               LogoutUri));
         }
      }

      private async Task LogoutAsync() {
         bool success = false;
         try {
            success = await Client.LogoutAsync();
         } catch (RestApiCallException ex) {
            await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
         }

         InitializeMenuItems();
      }

      private MenuItemModel GetMenuItem(string title, Uri navigationUri, string group = null) {
         return new MenuItemModel() {
            Title = title,
            Group = group,
            NavigationUri = navigationUri
         };
      }
   }
}
