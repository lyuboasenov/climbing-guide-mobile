using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Services {
   public class NavigationService : INavigationService {

      private App App { get; } = (App)App.Current;
      private Prism.Navigation.INavigationService InternalNavigationService { get; set; }

      public NavigationService(Prism.Navigation.INavigationService navigationService) {
         InternalNavigationService = navigationService;
      } 

      private void SubscribeToMessages() {
         MessagingCenter.Subscribe<ContentPage>(App, "GoBackToMainPage", (m) => {
            Device.BeginInvokeOnMainThread(() => {
               //App.MainPage = GetNavigationContainer();
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

      public async Task<INavigationResult> GoBackAsync() {
         return await InternalNavigationService.GoBackAsync();
      }

      public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters) {
         return await InternalNavigationService.GoBackAsync(parameters);
      }

      public async Task<INavigationResult> NavigateAsync(Uri uri) {
         return await InternalNavigationService.NavigateAsync(uri);
      }

      public async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters) {
         return await InternalNavigationService.NavigateAsync(uri, parameters);
      }

      public async Task<INavigationResult> NavigateAsync(string name) {
         return await InternalNavigationService.NavigateAsync(name);
      }

      public async Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters) {
         return await InternalNavigationService.NavigateAsync(name, parameters);
      }

      public Uri GetNavigationUri(string absolutePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, absolutePath);
      }

      public Uri GetShellNavigationUri(string relativePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{nameof(Views.Shell)}/NavigationPage/{relativePath}");
      }

      public INavigationParameters GetParameters(params KeyValuePair<string, object>[] parameters) {
         var result = new NavigationParameters();
         foreach(var parameter in parameters) {
            result.Add(parameter.Key, parameter.Value);
         }

         return result;
      }

      internal class Commands {
         public const string EXIT = "application-exit";
      }
   }
}
