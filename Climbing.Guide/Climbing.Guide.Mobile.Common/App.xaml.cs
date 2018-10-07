using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FreshMvvm;
using Climbing.Guide.Mobile.Common.ViewModels;

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

         var masterDetailNav = new FreshMasterDetailNavigationContainer();
         masterDetailNav.Init("Menu");
         masterDetailNav.AddPage<HomeViewModel>(HomeViewModel.VmTitle);
         // masterDetailNav.AddPage<RoutesMapPageModel>("Routes");
         // masterDetailNav.AddPage<TimerTrainingListPageModel>("Timed trainings");
         // masterDetailNav.AddPage<TimerSetupPageModel>("Timer");
         masterDetailNav.AddPage<AboutViewModel>(AboutViewModel.VmTitle);
         MainPage = masterDetailNav;

         // Used for navigation from a page with no masternavigation back to the master navigation template
         //MessagingCenter.Subscribe<Pages.Routes.RoutesMapPage>(this, "GoBackToMainPage", (m) => {
         //   Device.BeginInvokeOnMainThread(() => {
         //      MainPage = masterDetailNav;
         //   });
         //});
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
