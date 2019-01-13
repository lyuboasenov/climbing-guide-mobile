using Climbing.Guide.Forms.Helpers;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.GeoLocation;
using Climbing.Guide.Forms.Services.Navigation;
using System.Collections.Generic;
using System;

namespace Climbing.Guide.Forms.ViewModels.Content.List {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MapGuideViewModel : BaseGuideViewModel<MapGuideViewModel.Parameters> {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      public static NavigationRequest GetNavigationRequest(Navigation navigation) {
         return GetNavigationRequest(navigation, new Parameters());
      }

      public static NavigationRequest GetNavigationRequest(Navigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.List.MapGuideView), parameters);
      }

      private static int[] ZoomLevel { get; } = new int[] { 5000000, 5000000, 22000, 1400, 170, 170, 170 };

      public ICommand PinTappedCommand { get; private set; }
      public ICommand TraverseBackCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }
      public IEnumerable Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      private Progress Progress { get; }
      private GeoLocation GeoLocation { get; }

      public MapGuideViewModel(IApiClient client,
         IExceptionHandler errors,
         Navigation navigation,
         Alerts alerts,
         Media media,
         Progress progress,
         GeoLocation geoLocation) : base(client, errors, media, alerts, navigation) {
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

      protected async override Task TraverseToAsync(Area parentArea) {
         using (var loading = await Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(parentArea);
            (TraverseBackCommand as Command).ChangeCanExecute();

            Pins = Items;

            decimal latitude, longitude;
            if (null == parentArea) {
               var location = await GeoLocation.GetCurrentOrDefaultAsync();
               latitude = (decimal)location.Latitude;
               longitude = (decimal)location.Longitude;
            } else {
               latitude = parentArea.Latitude;
               longitude = parentArea.Longitude;
            }

            int zoomLevel = ZoomLevel[TraversalPath.Count];
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapHelper.GetPosition(latitude, longitude),
               new Distance(zoomLevel));
         }
      }

      private void InitializeCommands() {
         PinTappedCommand = new Command(async (data) => { await OnPinTapped(data); } );
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalPath.Count > 1);
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private Task InitializeData(Parameters parameters) {
         try {
            if (null != parameters.TraversalPath) {
               foreach (var area in parameters.TraversalPath) {
                  TraversalStack.Push(area);
                  TraversalPath.Add(area);
               }
            }
            return Task.CompletedTask;
         } catch (Exception ex) {
            return Errors.HandleAsync(ex);
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
         (TraverseBackCommand as Command).ChangeCanExecute();
      }

      private Task OnAddAsync() {
         return AddItemAsync();
      }

      private async Task ViewRoute(Route route) {
         if (null != route) {
            await Navigation.NavigateAsync(
               View.RouteViewModel.GetNavigationRequest(
                  Navigation,
                  new View.RouteViewModel.Parameters() {
                     Route = route
                  }));
         }
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
      }
   }
}