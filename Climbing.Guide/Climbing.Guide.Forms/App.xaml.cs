using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Climbing.Guide.Forms.Services;
using System;
using Climbing.Guide.Caching;
using Climbing.Guide.Services;
using Climbing.Guide.Tasks;
using Climbing.Guide.Serialization;
using Climbing.Guide.Caching.FileSystem;

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
         containerRegistry.Register<ITaskRunner, TaskRunner>();
         containerRegistry.Register<ICache, Cache>();
         containerRegistry.Register<ICacheRepository, Caching.Sqlite.SqliteCacheRepository>();
         containerRegistry.Register<IResourceService, ResourceService>();
         containerRegistry.Register<Serialization.ISerializer, Serialization.JsonSerializer>();
         containerRegistry.Register<Core.Api.IApiClient, ApiClient>();
         containerRegistry.Register<IEnvironment, Services.Environment>();

#if DEBUG
         containerRegistry.Register<Logging.ILogger, Logging.DebugLogger>();
#elif RELEASE
         containerRegistry.Register<Logging.ILogger, Logging.VoidLoggingService>();
#endif

         // Register instances
         containerRegistry.RegisterInstance(DependencyService.Get<IProgressService>());
         containerRegistry.RegisterInstance(Plugin.Media.CrossMedia.Current);

         containerRegistry.RegisterInstance(GetCacheSettings());
         containerRegistry.RegisterInstance(GetApiClientSettings());
      }

      private Core.Api.IApiClientSettings GetApiClientSettings() {
         ICache responseCache = this.Container.Resolve<ICache>();
         ICache largeResponseCache = new Cache(new FileSystemCacheRepository(GetLargeCacheSettings()), new JsonSerializer());

         var baseUrl = "https://api.climbingguide.org";
#if DEBUG
         baseUrl = "http://10.0.2.2:8000";
#endif

         var httpClientHandler = new Http.CachingHttpClientHandler(responseCache, largeResponseCache) {
            CachePeriod = TimeSpan.FromMinutes(1)
         };

         return new Core.Api.ApiClientSettings() {
            HttpClient = new System.Net.Http.HttpClient(httpClientHandler) {
               BaseAddress = new Uri(baseUrl)
            }
         };
      }

      private ICacheSettings GetCacheSettings() {
         var environment = IoC.Container.Get<IEnvironment>();

         var cacheLocation = System.IO.Path.Combine(environment.CachePath, "sqlite/");
         return new CacheSettings(cacheLocation);
      }

      private ICacheSettings GetLargeCacheSettings() {
         var environment = IoC.Container.Get<IEnvironment>();

         var cacheLocation = System.IO.Path.Combine(environment.CachePath, "files/");
         return new CacheSettings(cacheLocation);
      }
   }
}
