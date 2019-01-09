using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Forms.Models;
using Climbing.Guide.Forms.Services;
using System;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Forms.Services.Progress;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ShellMenuViewModel : BaseViewModel {
      private IApiClient Client { get; }
      private IExceptionHandler Errors { get; }
      private Navigation Navigation { get; }
      private Services.Events Events { get; }
      private Progress Progress { get; }

      public ObservableCollection<MenuItemModel> MenuItems { get; set; }
      public MenuItemModel SelectedMenuItem { get; set; }

      private Uri LogoutUri { get; } = UriHelper.Get(UriHelper.Schema.act, "Logout");
      private Uri TestUri { get; } = UriHelper.Get(UriHelper.Schema.act, "Test");
      

      public ShellMenuViewModel(IApiClient client,
         IExceptionHandler errors,
         Navigation navigation,
         Services.Events events,
         Progress progress) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Events = events;
         Progress = progress;

         Title = Resources.Strings.Main.CG;

         InitializeMenuItems();

         Events.GetEvent<Events.ShellMenuInvalidatedEvent>().Subscribe(InitializeMenuItems);
      }

      public async void OnSelectedMenuItemChanged() {
         await OnSelectedMenuItemChangedAsync();
      }

      public async Task OnSelectedMenuItemChangedAsync() {
         try {
            if (null != SelectedMenuItem) {
               if (SelectedMenuItem.NavigationUri == LogoutUri) {
                  await LogoutAsync();
               } else if (SelectedMenuItem.NavigationUri == TestUri) {
                  await TestAsync();
               } else {
                  var result = Navigation.NavigateAsync(SelectedMenuItem.NavigationUri);
                  result.Wait();
                  if (!result.Result.Result) {
                     await Errors.HandleAsync(result.Exception,
                        Resources.Strings.Main.Shell_Navigation_Error_Message,
                        SelectedMenuItem.Title);
                  }
               }
            } else if (MenuItems.Count > 0) {
               SelectedMenuItem = MenuItems[0];
            }
         }catch (Exception ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Shell_Navigation_Error_Message,
               SelectedMenuItem.Title);
         }
      }

      public void InitializeMenuItems() {
         MenuItems = new ObservableCollection<MenuItemModel>();

         var userLoggedIn = Client.AuthenticationManager.IsLoggedIn;

         MenuItems.Add(GetMenuItem(HomeViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.HomeView))));

         MenuItems.Add(GetMenuItem(Guide.GuideViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.Guide.GuideView)),
            Resources.Strings.Guide.Section_Title));

         MenuItems.Add(GetMenuItem(Guide.ExploreViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.Guide.ExploreView)),
            Resources.Strings.Guide.Section_Title));

         MenuItems.Add(GetMenuItem(Guide.SearchViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.Guide.SearchView)),
            Resources.Strings.Guide.Section_Title));

         if (userLoggedIn) {
            MenuItems.Add(GetMenuItem(User.ProfileViewModel.VmTitle,
               Navigation.GetShellNavigationUri(nameof(Views.User.ProfileView)),
               Resources.Strings.User.Section_Title));
         } else {
            MenuItems.Add(GetMenuItem(User.LoginViewModel.VmTitle,
               Navigation.GetShellNavigationUri(nameof(Views.User.LoginView)),
               Resources.Strings.User.Section_Title));
         }

         MenuItems.Add(GetMenuItem(Settings.SettingsViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.Settings.SettingsView)),
            Resources.Strings.User.Section_Title));

         MenuItems.Add(GetMenuItem(AboutViewModel.VmTitle,
            Navigation.GetShellNavigationUri(nameof(Views.AboutView))));

         if (userLoggedIn) {
            MenuItems.Add(GetMenuItem(Resources.Strings.User.Logout_Title,
               LogoutUri));
         }
#if DEBUG
         MenuItems.Add(GetMenuItem("Test initiation",
            TestUri));
#endif
      }

      private async Task TestAsync() {
         using (var progress = await Progress.CreateProgressSessionAsync()) {
            for (int i = 0; i < 100; i++) {
               await progress.UpdateProgressAsync(i, 100, $"{i} / 100 items processed.");
               await Task.Delay(100);
            }
         }
         //var progressService = GetService<IProgressService>();
         //await progressService.ShowLoadingIndicatorAsync();

         //for (int i = 0; i < 50; i++) {
         //   await Task.Delay(500);
         //}

         //await progressService.HideLoadingIndicatorAsync();
      }

      private async Task LogoutAsync() {
         try {
            await Client.AuthenticationManager.LogoutAsync();
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Communication_Error_Message,
               Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
         }

         InitializeMenuItems();

         await Navigation.NavigateAsync(Navigation.GetShellNavigationUri(nameof(Views.HomeView)));
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
