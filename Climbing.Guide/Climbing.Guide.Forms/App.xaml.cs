using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Climbing.Guide.Forms.Services;
using System;
using Climbing.Guide.Caching;
using Climbing.Guide.Tasks;
using Climbing.Guide.Serialization;
using Climbing.Guide.Caching.FileSystem;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Api;
using Climbing.Guide.Http;
using System.Net.Http;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Forms.Services.GeoLocation;
using Alat.Validation;
using Plugin.Iconize;
using Alat.Caching;
using Alat.Caching.Serialization;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Forms {
   public partial class App {
#if DEBUG
      private string BaseUrl { get; } = "http://10.0.2.2:8000";
      // private string BaseUrl { get; } = "http://127.0.0.1:8000";
#else
      private string BaseUrl { get; } = "https://api.climbingguide.org";
#endif
      public App() : this(null) { }

      public App(IPlatformInitializer initializer) : base(initializer) { }

      protected override async void OnInitialized() {
         InitializeComponent();

         // HACK: register Prism.Navigation.INavigationService in order to be used in
         // NavigationService creation
         (Container as IContainerExtension).RegisterInstance(NavigationService);

         await NavigationService.NavigateAsync(
            Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{nameof(Views.Shell)}/IconNavigationPage/{nameof(Views.HomeView)}"));
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
         containerRegistry.RegisterForNavigation<IconNavigationPage>();

         foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.FullName.Contains(".Views.") && type.IsSubclassOf(typeof(Page))) {
               containerRegistry.RegisterForNavigation(type, type.Name);
            }
         }
      }

      private void RegisterServices(IContainerRegistry containerRegistry) {
         // Register services
         containerRegistry.Register<Services.Events, Services.Impl.Events>();
         containerRegistry.Register<Preferences, Services.Impl.Preferences>();
         containerRegistry.Register<IExceptionHandler, Services.Impl.FormsExceptionHandler>();
         containerRegistry.Register<Alerts, Services.Impl.Alerts>();
         containerRegistry.Register<Media, Services.Impl.Media>();
         containerRegistry.Register<IAsyncTaskRunner, Services.Impl.FormsTaskRunner>();
         containerRegistry.Register<ISyncTaskRunner, Services.Impl.FormsTaskRunner>();
         containerRegistry.Register<IMainThreadTaskRunner, Services.Impl.FormsTaskRunner>();
         containerRegistry.Register<Cache, Alat.Caching.Impl.Cache>();
         containerRegistry.Register<CacheStore, Caching.Sqlite.SqliteCacheStore>();
         containerRegistry.Register<Resource, Services.Impl.Resources>();
         containerRegistry.Register<Serializer, JsonSerializer>();
         containerRegistry.Register<Services.Environment, Services.Impl.Environment>();
         containerRegistry.Register<Progress, Services.Progress.Impl.Progress>();
         containerRegistry.Register<GeoLocation, Services.GeoLocation.Impl.GeoLocation>();

#if DEBUG
         containerRegistry.Register<Alat.Logging.Logger, Alat.Logging.DebugLogger>();
#elif RELEASE
         containerRegistry.Register<Alat.Logging.Logger, Alat.Logging.VoidLogger>();
#endif

         // Register instances
         containerRegistry.RegisterSingleton<IApiClient, ApiClient>();
         containerRegistry.RegisterSingleton<Services.Navigation.Navigation, Services.Navigation.Impl.Navigation>();
         containerRegistry.RegisterSingleton<ValidationContextFactory, Alat.Validation.Impl.ValidationContextFactory>();

         containerRegistry.RegisterInstance(Plugin.Media.CrossMedia.Current);

         containerRegistry.RegisterInstance(GetCacheSettings());

         GetApiClientSettings(containerRegistry);
      }

      private void GetApiClientSettings(IContainerRegistry containerRegistry) {

         Cache responseCache = this.Container.Resolve<Cache>();
         Cache largeResponseCache = new Alat.Caching.Impl.Cache(new FileSystemCacheStore(GetLargeCacheSettings()), new JsonSerializer());

         var cachingHttpClientManager = new Http.CachingHttpClientManager();
         containerRegistry.RegisterInstance<Http.ICachingHttpClientManager>(cachingHttpClientManager);

         var httpClient = new HttpClient() { BaseAddress = new Uri(BaseUrl) };
         var authenticationManager = new Api.ClimbingGuideAuthenticationManager(httpClient);
         containerRegistry.RegisterInstance<IAuthenticationManager>(authenticationManager);

         containerRegistry.RegisterInstance<IApiClientSettings>(
            new ApiClientSettings(() => {
               return new HttpClient(
                  new OAuthHandler(authenticationManager,
                     new RetryingHandler(3,
                        new CachingHandler(cachingHttpClientManager, responseCache, largeResponseCache)))) {
                     BaseAddress = new Uri(BaseUrl)
               };
            }, authenticationManager));
      }

      private FileSystemCacheSettings GetCacheSettings() {
         var environment = IoC.Container.Get<Services.Environment>();

         var cacheLocation = System.IO.Path.Combine(environment.CachePath, "sqlite/");
         return new FileSystemCacheSettings(cacheLocation);
      }

      private FileSystemCacheSettings GetLargeCacheSettings() {
         var environment = IoC.Container.Get<Services.Environment>();

         var cacheLocation = System.IO.Path.Combine(environment.CachePath, "files/");
         return new FileSystemCacheSettings(cacheLocation);
      }
   }
}
