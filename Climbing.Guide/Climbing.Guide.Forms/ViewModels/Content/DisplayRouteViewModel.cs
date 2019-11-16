using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Forms.Queries;
using Climbing.Guide.Forms.Services.Environment;
using Climbing.Guide.Forms.Services.Navigation;
using Climbing.Guide.Forms.ViewModels.Content.List;
using Prism.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.ViewModels.Content {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class DisplayRouteViewModel : ParametrisedBaseViewModel<DisplayRouteViewModel.Parameters>, IDestructible {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public static INavigationRequest GetNavigationRequest(
         Services.Navigation.INavigation navigation,
         Parameters parameters) {
         var displayViewRequest = navigation.GetNavigationRequest(nameof(Views.Content.DisplayRouteView), parameters);
         return navigation.GetNavigationRequest("IconNavigationPage", parameters, displayViewRequest);
      }

      private IApiClient Client { get; }
      private IEnvironment Environment { get; }
      private Services.Navigation.INavigation Navigation { get; }
      private IQueryFactory QueryFactory { get; }

      public ICommand TraverseBackCommand { get; private set; }
      public Route Route { get; set; }
      public ObservableCollection<Area> TraversalPath { get; set; }
      public ICommand ViewSchemaCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public IEnumerable Pins { get; set; }

      public DisplayRouteViewModel(IApiClient client,
         Services.Navigation.INavigation navigation,
         IEnvironment environment,
         IQueryFactory commandQueryFactory) {
         Client = client ?? throw new ArgumentNullException(nameof(client));
         Navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
         Environment = environment ?? throw new ArgumentNullException(nameof(environment));
         QueryFactory = commandQueryFactory ?? throw new ArgumentNullException(nameof(commandQueryFactory));

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();

         InitializeCommands();
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeAsync(parameters.Route, parameters.TraversalPath);
      }

      private async Task InitializeAsync(Route route, IEnumerable<Area> traversalPath) {
         Route = route ?? throw new ArgumentNullException(nameof(route));
         if (null == traversalPath) {
            throw new ArgumentNullException(nameof(traversalPath));
         }
         if (!traversalPath.Any()) {
            throw new ArgumentException(nameof(traversalPath));
         }

         foreach (var area in traversalPath) {
            TraversalPath.Add(area);
         }

         Title = Route.Name;

         var tempFile = Environment.GetTempFileName();
         await Client.DownloadAsync(Route.Schema_1024, tempFile, true).ContinueWith((_) => LocalSchemaThumbPath = tempFile);

         SchemaRoute = new ObservableCollection<Point>() {
            new Point(0, 0), new Point(0.7, 0), new Point(0.7, 0.7), new Point(1, 1)
         };

         VisibleRegion = MapSpan.FromCenterAndRadius(
               MapHelper.GetPosition(route.Latitude, route.Longitude),
               new Distance(170));

         Pins = new[] { route };
      }

      private void InitializeCommands() {
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync());
      }

      private async Task OnTraverseBackAsync() {
         var parameters = new ListGuideViewModel.Parameters() { TraversalPath = TraversalPath };
         var navigationRequest = 
            ListGuideViewModel.GetNavigationRequest(
               Navigation,
               parameters);
         navigationRequest = Navigation.GetNavigationRequest("IconNavigationPage", parameters, navigationRequest);
         await Navigation.NavigateAsync(navigationRequest);
      }

      private void CleanUp() {
         if (File.Exists(LocalSchemaThumbPath)) {
            File.Delete(LocalSchemaThumbPath);
         }
      }

      public void Destroy() {
         CleanUp();
      }

      public class Parameters {
         public Route Route { get; set; }
         public IEnumerable<Area> TraversalPath { get; set; }

         public Parameters(Route route, IEnumerable<Area> traversalPath) {
            Route = route ?? throw new ArgumentNullException(nameof(route));
            TraversalPath = traversalPath ?? throw new ArgumentNullException(nameof(traversalPath));

            if (!traversalPath.Any()) {
               throw new ArgumentException(nameof(traversalPath));
            }
         }
      }
   }
}