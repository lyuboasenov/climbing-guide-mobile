using Climbing.Guide.Forms.Helpers;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Climbing.Guide.Api.Schemas;
using System;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Tasks;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;

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
         IProgress progress, 
         ISyncTaskRunner syncTaskRunner) : base(client, errors, syncTaskRunner) {
         Progress = progress;
         Navigation = navigation;

         Title = VmTitle;

         InitializeCommands();
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         try {
            await Progress.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
            await InitializeViewModel();
         } finally {
            await Progress.HideLoadingIndicatorAsync();
         }
      }

      protected override async Task InitializeAreasAsync(Area parentArea) {
         await base.InitializeAreasAsync(parentArea);

         Pins = Areas;

         int zoomLevel = ZoomLevel[Breadcrumbs.Count];
         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedArea.Latitude, SelectedArea.Longitude),
            new Distance(zoomLevel));
      }

      private void InitializeCommands() {
         PinTappedCommand = new Command(async (data) => { await OnPinTapped(data); } );
      }

      private async Task InitializeViewModel() {
         Location position = null;
         try {
            position = await Geolocation.GetLocationAsync();
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

         if (null != position) {
            VisibleRegion = MapSpan.FromCenterAndRadius(
            new Position(position.Latitude, position.Longitude),
            new Distance(5000000));
         }
      }

      private async Task OnPinTapped(object data) {
         if (data is Area) {
            SelectedArea = data as Area;
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