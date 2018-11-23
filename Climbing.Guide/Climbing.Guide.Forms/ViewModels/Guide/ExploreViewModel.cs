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

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExploreViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      //public ObservableCollection<Pin> Pins { get; set; }
      public ICommand PinTappedCommand { get; private set; }
      public IEnumerable Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      public override bool AutoSelectRegions { get; } = false;
      public override bool AutoSelectAreas { get; } = false;
      public override bool AutoSelectSectors { get; } = false;
      public override bool AutoSelectRoutes { get; } = false;

      public ExploreViewModel() {
         Title = VmTitle;
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         var progressService = GetService<IProgressService>();
         try {
            await progressService.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
         } finally {
            await progressService.HideLoadingIndicatorAsync();
         }
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         PinTappedCommand = new Command(async (data) => { await OnPinTapped(data); } );
      }

      private async Task OnPinTapped(object data) {
         if (data is Api.Schemas.Region) {
            SelectedRegion = data as Api.Schemas.Region;
         } else if (data is Area) {
            SelectedArea = data as Area;
         } else if (data is Sector) {
            SelectedSector = data as Sector;
         } else if (data is Route) {
            await ViewRoute(data as Route);
         }
      }

      protected async override Task InitializeRegionsAsync() {
         await base.InitializeRegionsAsync();
         Pins = Regions;

         Location position = null;
         try {
            position = await Geolocation.GetLocationAsync();
         } catch (FeatureNotSupportedException fnsEx) {
            await Errors.HandleExceptionAsync(fnsEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (PermissionException pEx) {
            await Errors.HandleExceptionAsync(pEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (Exception ex) {
            await Errors.HandleExceptionAsync(ex,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         }

         if (null != position) {
            VisibleRegion = MapSpan.FromCenterAndRadius(
            new Position(position.Latitude, position.Longitude),
            new Distance(5000000));
         }
      }

      public override void OnSelectedRegionChanged() {
         base.OnSelectedRegionChanged();
         Pins = Areas;

         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedRegion.Latitude, SelectedRegion.Longitude),
            new Distance(22000));
      }

      public override void OnSelectedAreaChanged() {
         base.OnSelectedAreaChanged();
         Pins = Sectors;

         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedArea.Latitude, SelectedArea.Longitude),
            new Distance(1400));
      }

      public override void OnSelectedSectorChanged() {
         base.OnSelectedSectorChanged();
         Pins = Routes;
         
         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedSector.Latitude, SelectedSector.Longitude),
            new Distance(170));
      }

      private async Task ViewRoute(Route route) {
         if (null != route) {
            var navigationResult = await Navigation.NavigateAsync(
               Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
               route);
            if (!navigationResult.Result) {
               await Errors.HandleExceptionAsync(navigationResult.Exception,
                  Resources.Strings.Routes.Route_View_Error_Message, route.Name);
            }
         }
      }
   }
}