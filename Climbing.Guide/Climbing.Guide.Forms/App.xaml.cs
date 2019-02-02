using Alat.Caching;
using Alat.Caching.FileSystem;
using Alat.Http;
using Alat.Http.Caching;
using Alat.Http.Caching.Sessions;
using Alat.Logging;
using Alat.Logging.Factories;
using Alat.Validation;
using Climbing.Guide.Api;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Commands;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.GeoLocation;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Tasks;
using Plugin.Iconize;
using Prism;
using Prism.Ioc;
using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Climbing.Guide.Forms {
   public partial class App {
#if DEBUG
      private string BaseUrl { get; } = "http://10.0.2.2:8000";
#else
      private string BaseUrl { get; } = "https://api.climbingguide.org";
#endif
      public App() : this(null) { }

      public App(IPlatformInitializer initializer) : base(initializer) { }

      protected override async void OnInitialized() {
         InitializeComponent();

         // HACK: register Prism.Navigation.INavigationService in order to be used in
         // NavigationService creation
         (Container as IContainerExtension)?.RegisterInstance(NavigationService);

         await NavigationService.NavigateAsync(
            Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav,
               $"{nameof(Views.Shell)}/IconNavigationPage/{nameof(Views.HomeView)}"));
      }

      protected override IContainerExtension CreateContainerExtension() {
         return new Services.IoC.Container(base.CreateContainerExtension());
      }

      protected override void ConfigureViewModelLocator() {
         base.ConfigureViewModelLocator();

         Prism.Mvvm.ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelTypeResolver);
      }

      private static Type ViewTypeToViewModelTypeResolver(Type type) {
         var viewName = type.FullName;
         if (String.IsNullOrEmpty(viewName)
            || !viewName.StartsWith("Climbing.Guide")) {
            return null;
         }

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

      private static void RegisterNavigation(IContainerRegistry containerRegistry) {
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
         containerRegistry.Register<Services.Preferences.IPreferences, Services.Preferences.Preferences>();
         containerRegistry.Register<Services.Exceptions.IExceptionHandler, Services.Exceptions.FormsExceptionHandler>();
         containerRegistry.Register<Services.Alerts.IAlerts, Services.Alerts.Alerts>();
         containerRegistry.Register<Services.Media.IMedia, Services.Media.Media>();
         containerRegistry.Register<IAsyncTaskRunner, Services.Tasks.FormsTaskRunner>();
         containerRegistry.Register<ISyncTaskRunner, Services.Tasks.FormsTaskRunner>();
         containerRegistry.Register<IMainThreadTaskRunner, Services.Tasks.FormsTaskRunner>();
         containerRegistry.Register<Alat.Caching.ICache, FileSystemCache>();
         containerRegistry.Register<Services.Environment.IEnvironment, Services.Environment.Environment>();
         containerRegistry.Register<IProgress, Progress>();
         containerRegistry.Register<IGeoLocation, GeoLocation>();

#if DEBUG
         containerRegistry.RegisterInstance(LoggerFactory.GetDebugLogger(Level.All));
#elif RELEASE
         containerRegistry.RegisterInstance(LoggerFactory.GetDisabledLogger());
#endif

         // Register instances
         containerRegistry.RegisterSingleton<IApiClient, ApiClient>();
         containerRegistry.RegisterSingleton<Services.Navigation.INavigation, Services.Navigation.Navigation>();
         containerRegistry.RegisterSingleton<IValidationContextFactory, ValidationContextFactory>();
         containerRegistry.RegisterSingleton<ICommandQueryFactory, CommandQueryFactory>();

         containerRegistry.RegisterInstance(Plugin.Media.CrossMedia.Current);
         containerRegistry.RegisterInstance<Services.IoC.IContainer>(containerRegistry as Services.IoC.IContainer);

         containerRegistry.RegisterInstance(GetCacheSettings());

         GetApiClientSettings(containerRegistry);
      }

      private void GetApiClientSettings(IContainerRegistry containerRegistry) {
         var responseCache = Container.Resolve<Alat.Caching.ICache>();

         var httpClient = new HttpClient() { BaseAddress = new Uri(BaseUrl) };
         var authenticationManager = new Api.ClimbingGuideAuthenticationManager(httpClient);
         containerRegistry.RegisterInstance<IAuthenticationManager>(authenticationManager);

         var sessionFactory = new SessionFactory();
         var cachingHandlerSettings = new Alat.Http.Caching.Settings {
            CachePeriod = TimeSpan.FromMinutes(5),
            CachingEnabledByDefault = false,
            Cache = new CachingHandlerCacheAdapter(responseCache)
         };

         containerRegistry.RegisterInstance<IApiClientSettings>(
            new ApiClientSettings(() => {
               return new HttpClient(
                  new OAuthHandler(authenticationManager,
                     new RetryingHandler(3,
                        new CachingHandler(sessionFactory, cachingHandlerSettings)))) {
                     BaseAddress = new Uri(BaseUrl)
               };
            }, authenticationManager));

         containerRegistry.RegisterInstance(sessionFactory);
      }

      private static FileSystemCacheSettings GetCacheSettings() {
         var environment = Services.IoC.Container.Get<Services.Environment.IEnvironment>();

         var cacheLocation = System.IO.Path.Combine(environment.CachePath, "files/");
         return new FileSystemCacheSettings(cacheLocation);
      }
   }
}
