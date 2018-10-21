using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FreshMvvm;
using System.Threading.Tasks;
using Climbing.Guide.Mobile.Common.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Mobile.Common {
   public partial class App : Application {

      public App() {
         InitializeComponent();

         // Show splash
         MainPage = new Views.SplashView();

         //Substitute builtin PageModelMapper with a custom one.
         FreshPageModelResolver.PageModelMapper = new ViewModelMapper();

         Task.Run(() => {
            try {
               RegisterServices();
               ServiceLocator.Get<INavigationService>().InitializeNavigation();
            } catch (System.Exception ex) {
               ServiceLocator.Get<IErrorService>().LogException(ex);
            }
         });
      }

      private void RegisterServices() {
         ServiceLocator.Register<IErrorService, ErrorService>();
         ServiceLocator.Register<IAlertService, AlertService>();
#if DEBUG
         ServiceLocator.Register<IRestApiClient>(new RestApiClient("http://10.0.2.2:8000"));
#elif RELEASE
         ServiceLocator.Register<IRestApiClient>(new RestApiClient("https://api.climbingguide.org"));
#endif
         ServiceLocator.Register<INavigationService, NavigationService>();
         ServiceLocator.Register<Core.Models.Routes.IGradeService, Core.Models.Routes.GradeService>();
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
