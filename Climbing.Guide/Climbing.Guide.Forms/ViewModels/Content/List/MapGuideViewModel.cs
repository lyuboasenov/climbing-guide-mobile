using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Forms.Services.Alerts;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.GeoLocation;
using Climbing.Guide.Forms.Services.Media;
using Climbing.Guide.Forms.Services.Navigation;
using Climbing.Guide.Forms.Services.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using INavigation = Climbing.Guide.Forms.Services.Navigation.INavigation;

namespace Climbing.Guide.Forms.ViewModels.Content.List {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MapGuideViewModel : BaseGuideViewModel<MapGuideViewModel.Parameters> {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      public static INavigationRequest GetNavigationRequest(INavigation navigation) {
         return GetNavigationRequest(navigation, new Parameters());
      }

      public static INavigationRequest GetNavigationRequest(INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.List.MapGuideView), parameters);
      }

      private static int[] ZoomLevel { get; } = new int[] { 5000000, 5000000, 22000, 1400, 170, 170, 170 };

      public ICommand PinTappedCommand { get; private set; }
      public ICommand TraverseBackCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }
      public IEnumerable Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      private IProgress Progress { get; }
      private IGeoLocation GeoLocation { get; }

      public MapGuideViewModel(IApiClient client,
         IExceptionHandler еxceptionHandler,
         INavigation navigation,
         IAlerts alerts,
         IMedia media,
         IProgress progress,
         IGeoLocation geoLocation) : base(client, еxceptionHandler, media, alerts, navigation) {
         Progress = progress;
         GeoLocation = geoLocation;

         Title = VmTitle;

         InitializeCommands();
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeData(parameters);

         Area parentArea = null;
         if (TraversalStack.Count > 0) {
            parentArea = TraversalStack.Pop();
            TraversalPath.Remove(parentArea);
         }

         await TraverseToAsync(parentArea);
      }

      protected async override Task TraverseToAsync(Area area) {
         using (var loading = await Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(area);
            (TraverseBackCommand as Command)?.ChangeCanExecute();

            Pins = Items;

            decimal latitude, longitude;
            if (area == null) {
               var location = await GeoLocation.GetCurrentOrDefaultAsync();
               latitude = (decimal)location.Latitude;
               longitude = (decimal)location.Longitude;
            } else {
               latitude = area.Latitude;
               longitude = area.Longitude;
            }

            int zoomLevel = ZoomLevel[TraversalPath.Count];
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapHelper.GetPosition(latitude, longitude),
               new Distance(zoomLevel));
         }
      }

      private void InitializeCommands() {
         PinTappedCommand = new Command(async (data) => await OnPinTapped(data));
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalPath.Count > 1);
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private Task InitializeData(Parameters parameters) {
         try {
            if (parameters.TraversalPath != null) {
               foreach (var area in parameters.TraversalPath) {
                  TraversalStack.Push(area);
                  TraversalPath.Add(area);
               }
            }
            return Task.CompletedTask;
         } catch (Exception ex) {
            return ExceptionHandler.HandleAsync(ex);
         }
      }

      private async Task OnPinTapped(object data) {
         if (data is Area) {
            await TraverseToAsync(data as Area);
         } else if (data is Route) {
            await ViewRoute(data as Route);
         }
      }

      private async Task OnTraverseBackAsync() {
         await TraverseBackAsync();
         (TraverseBackCommand as Command)?.ChangeCanExecute();
      }

      private Task OnAddAsync() {
         return AddItemAsync();
      }

      private async Task ViewRoute(Route route) {
         if (route != null) {
            await Navigation.NavigateAsync(
               DisplayRouteViewModel.GetNavigationRequest(
                  Navigation,
                  new DisplayRouteViewModel.Parameters() {
                     Route = route
                  }));
         }
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
      }
   }
}