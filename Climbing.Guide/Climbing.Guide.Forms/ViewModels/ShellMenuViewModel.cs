using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Models;
using System;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Forms.Services.Navigation;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ShellMenuViewModel : BaseViewModel {
      private IApiClient Client { get; }
      private IExceptionHandler Errors { get; }
      private INavigation Navigation { get; }
      private Services.IEvents Events { get; }
      private IProgress Progress { get; }

      public ObservableCollection<MenuItemModel> MenuItems { get; set; }
      public MenuItemModel SelectedMenuItem { get; set; }

      private MenuItemModel LogoutMenuItem { get; } = 
         new MenuItemModel() { Title = Resources.Strings.User.Logout_Title };
      private MenuItemModel TestMenuItem { get; } = new MenuItemModel() { Title = "Test initiation" };


      public ShellMenuViewModel(IApiClient client,
         IExceptionHandler errors,
         INavigation navigation,
         Services.IEvents events,
         IProgress progress) {
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

         await Navigation.NavigateAsync(HomeViewModel.GetNavigationRequest(Navigation));
      }

      private MenuItemModel GetMenuItem(string title, INavigationRequest navigationRequest, string group = null) {
         return new MenuItemModel() {
            Title = title,
            Group = group,
            NavigationRequest = Navigation.GetNavigationRequest("IconNavigationPage", navigationRequest)
         };
      }
   }
}
