using Climbing.Guide.Forms.Helpers;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Climbing.Guide.Api.Schemas;
using System;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services.Progress;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExploreViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;
      private static int[] ZoomLevel { get; } = new int[] { 5000000, 22000, 1400, 170 };

      private IProgress Progress { get; }
      private Services.INavigation Navigation { get; }

      public ICommand PinTappedCommand { get; private set; }
      public IEnumerable Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      public ExploreViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation,
         IProgress progress) : base(client, errors) {
         Progress = progress;
         Navigation = navigation;

         Title = VmTitle;

         InitializeCommands();
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         await TraverseToAsync(null);
      }

      protected async override Task TraverseToAsync(Area parentArea) {
         using (var loading = Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(parentArea);

            Pins = Items;

            decimal latitude, longitude;
            if (null == parentArea) {
               var location = await GetCurrentLocation();
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
      }

      private async Task<Location> GetCurrentLocation() {
         try {
            return await Geolocation.GetLocationAsync();
         } catch (FeatureNotSupportedException fnsEx) {
            await Errors.HandleAsync(fnsEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (PermissionException pEx) {
            await Errors.HandleAsync(pEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (Exception ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         }

         return await Geolocation.GetLastKnownLocationAsync();
      }

      private async Task OnPinTapped(object data) {
         if (data is Area) {
            await TraverseToAsync(data as Area);
         } else if (data is Route) {
            await ViewRoute(data as Route);
         }
      }

      private async Task ViewRoute(Route route) {
         if (null != route) {
            var navigationResult = await Navigation.NavigateAsync(
               Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
               route);
            if (!navigationResult.Result) {
               await Errors.HandleAsync(navigationResult.Exception,
                  Resources.Strings.Routes.Route_View_Error_Message, route.Name);
            }
         }
      }
   }
}