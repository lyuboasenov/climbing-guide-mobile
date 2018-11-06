using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Climbing.Guide.Forms.Services;
using System;
using Climbing.Guide.Caching;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Forms {
   public partial class App {

      public App() : this(null) { }

      public App(IPlatformInitializer initializer) : base(initializer) { }

      protected override async void OnInitialized() {
         InitializeComponent();

         // HACK: register Prism.Navigation.INavigationService in order to be used in
         // NavigationService creation
         (Container as IContainerExtension).RegisterInstance<Prism.Navigation.INavigationService>(NavigationService);

         await NavigationService.NavigateAsync(
            Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{nameof(Views.Shell)}/NavigationPage/{nameof(Views.HomeView)}"));
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

         RegisterServices(containerRegistry);
      }

      private void RegisterNavigation(IContainerRegistry containerRegistry) {
         containerRegistry.RegisterForNavigation<NavigationPage>();
         foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.FullName.Contains(".Views.") && type.IsSubclassOf(typeof(Page))) {
               containerRegistry.RegisterForNavigation(type, type.Name);
            }
         }
      }

      private void RegisterServices(IContainerRegistry containerRegistry) {
         // Register services
         containerRegistry.Register<IEventService, EventService>();
         containerRegistry.Register<IPreferenceService, PreferenceService>();
         containerRegistry.Register<IErrorService, ErrorService>();
         containerRegistry.Register<IAlertService, AlertService>();
         containerRegistry.Register<Core.Models.Routes.IGradeService, Core.Models.Routes.GradeService>();
         containerRegistry.Register<INavigationService, NavigationService>();
         containerRegistry.Register<IMediaService, MediaService>();
         containerRegistry.Register<ICache, Cache>();
         containerRegistry.Register<ICacheRepository, Caching.Sqlite.SqliteCacheRepository>();
         containerRegistry.Register<IResourceService, ResourceService>();
         containerRegistry.Register<Serialization.ISerializer, Serialization.JsonSerializer>();

#if DEBUG
         containerRegistry.RegisterInstance<IApiClient>(new RestApiClient("http://10.0.2.2:8000"));
         containerRegistry.Register<Logging.ILoggingService, Logging.DebugLoggingService>();
#elif RELEASE
         containerRegistry.RegisterInstance<IApiClient>(new RestApiClient("https://api.climbingguide.org"));
         containerRegistry.Register<Logging.ILoggingService, Logging.VoidLoggingService>();
#endif
         // Register instances
         containerRegistry.RegisterInstance(DependencyService.Get<IProgressService>());
         containerRegistry.RegisterInstance(Plugin.Media.CrossMedia.Current);

         var cacheLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache/sqlite/");
         containerRegistry.RegisterInstance<ICacheSettings>(new CacheSettings(cacheLocation));
      }
   }
}
