using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FreshMvvm;
using Climbing.Guide.Core.API;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Mobile.Common {
   public partial class App : Application {

      public App() {
         InitializeComponent();

         // Show splash
         MainPage = new Views.SplashView();

         //Set theme to light
         //var themeResources = typeof(Xamarin.Forms.Themes.DarkThemeResources);
         //App.Current.Resources = new ResourceDictionary { MergedWith = themeResources };

         //Substitute builtin PageModelMapper with a custom one.
         FreshPageModelResolver.PageModelMapper = new ViewModelMapper();

         //FreshIOC.Container.Register<IDatabaseService, DatabaseService>();
         //FreshIOC.Container.Register<IUserDialogs>(UserDialogs.Instance);

         InitializeRestApiClient();

         Task.Run(NavigationManager.Current.InitializeNavigationAsync);
      }

      private void InitializeRestApiClient() {

         string token = string.Empty;
         string refreshToken = string.Empty;
         string username = string.Empty;

         Task.Run(async () => {
            try {
               token = await SecureStorage.GetAsync("token");
               refreshToken = await SecureStorage.GetAsync("refresh_token");
               username = await SecureStorage.GetAsync("username");
            } catch (Exception ex) {
               // Possible that device doesn't support secure storage on device.
               Console.WriteLine($"Error: {ex.Message}");
            }
         }).Wait();

         // TODO UPDATE api address 
         RestApiClient.UpdateRestApiClientSettings(
            new RestApiClientSettings() {
#if DEBUG
               BaseUrl = "http://10.0.2.2:8000",
#elif RELEASE
               BaseUrl = "http://10.0.2.2:8000",
#endif
               Token = token,
               RefreshToken = refreshToken,
               Username = username
            });
      }

      protected override void OnStart() {
         // Handle when your app starts
      }

      protected override void OnSleep() {
         // Handle when your app sleeps
      }

      protected override void OnResume() {
         // Handle when your app resumes
      }
   }
}
