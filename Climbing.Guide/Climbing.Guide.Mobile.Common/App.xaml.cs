using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FreshMvvm;
using Climbing.Guide.Core.API;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Mobile.Common {
   public partial class App : Application {

      public App() {
         InitializeComponent();

         //Set theme to light
         //var themeResources = typeof(Xamarin.Forms.Themes.DarkThemeResources);
         //App.Current.Resources = new ResourceDictionary { MergedWith = themeResources };

         //Substitute builtin PageModelMapper with a custom one.
         FreshPageModelResolver.PageModelMapper = new ViewModelMapper();

         //FreshIOC.Container.Register<IDatabaseService, DatabaseService>();
         //FreshIOC.Container.Register<IUserDialogs>(UserDialogs.Instance);

         // TODO UPDATE api address 
         RestApiClient.UpdateRestApiClientSettings(
            new RestApiClientSettings() {
#if DEBUG
               BaseUrl = "http://10.0.2.2:8000"
#elif RELEASE
               BaseUrl = "http://10.0.2.2:8000"
#endif
            });

         NavigationManager.InitializeNavigation();
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
