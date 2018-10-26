using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Climbing.Guide.Mobile.Common.Services;
using System;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Mobile.Common {
   public partial class App {

      public App() : this(null) { }

      public App(IPlatformInitializer initializer) : base(initializer) { }

      protected override async void OnInitialized() {
         InitializeComponent();

         // HACK: register Prism.Navigation.INavigationService in order to be used in
         // NavigationService creation
         (Container as IContainerExtension).RegisterInstance<Prism.Navigation.INavigationService>(NavigationService);

         await NavigationService.NavigateAsync(Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, "Shell/NavigationPage/HomeView"));
      }

      protected override IContainerExtension CreateContainerExtension() {
         return new IoC.Container(base.CreateContainerExtension());
      }

      protected override void ConfigureViewModelLocator() {
         base.ConfigureViewModelLocator();

         Prism.Mvvm.ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelTypeResolver);
      }

      private Type ViewTypeToViewModelTypeResolver(Type type) {
         var viewName = type.FullName;
         if (String.IsNullOrEmpty(viewName) ||
            !viewName.StartsWith("Climbing.Guide"))
            return null;

         string viewModelName = string.Empty;
         if (viewName.EndsWith("View")) {
            viewModelName = viewName + "Model";
         } else {
            viewModelName = viewName + "ViewModel";
         }
         viewModelName = viewModelName.Replace(".Views.", ".ViewModels.");

         return Type.GetType(viewModelName);
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry) {
         RegisterNavigation(containerRegistry);
         
         containerRegistry.Register<IActionService, ActionService>();
         containerRegistry.Register<IErrorService, ErrorService>();
         containerRegistry.Register<IAlertService, AlertService>();
#if DEBUG
         containerRegistry.RegisterInstance<IRestApiClient>(new RestApiClient("http://10.0.2.2:8000"));
#elif RELEASE
         containerRegistry.RegisterInstance<IRestApiClient>(new RestApiClient("https://api.climbingguide.org"));
#endif
         containerRegistry.Register<Core.Models.Routes.IGradeService, Core.Models.Routes.GradeService>();
         containerRegistry.Register<INavigationService, NavigationService>();
      }

      private void RegisterNavigation(IContainerRegistry containerRegistry) {
         containerRegistry.RegisterForNavigation<NavigationPage>();
         foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.FullName.Contains(".Views.") && type.IsSubclassOf(typeof(Page))) {
               containerRegistry.RegisterForNavigation(type, type.Name);
            }
         }

         //containerRegistry.RegisterForNavigation<Views.Shell>();
         

         //containerRegistry.RegisterForNavigation<Views.ShellMenu>();
         //containerRegistry.RegisterForNavigation<Views.HomeView>();
         //containerRegistry.RegisterForNavigation<Views.AboutView>();

      }
   }
}
