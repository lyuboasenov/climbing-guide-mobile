using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Forms.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class GuideViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public ObservableCollection<Core.API.Schemas.Region> Regions { get; set; }
      public ObservableCollection<Area> Areas { get; set; }
      public ObservableCollection<Sector> Sectors { get; set; }
      public ObservableCollection<Route> Routes { get; set; }

      public Core.API.Schemas.Region SelectedRegion { get; set; }
      public Area SelectedArea { get; set; }
      public Sector SelectedSector { get; set; }
      public Route SelectedRoute { get; set; }

      public ICommand ClearFilterCommand { get; private set; }
      public ICommand RouteTappedCommand { get; private set; }
      public ICommand AddRouteCommand { get; private set; }

      public GuideViewModel() {
         Title = VmTitle;

         InitializeRegions();
      }

      protected override void InitializeCommands() {
         ClearFilterCommand = new Command(() => {
            SelectedRoute = null;
            SelectedSector = null;
            SelectedArea = null;
            SelectedRegion = null;

            Areas = null;
            Sectors = null;
            Routes = null;
         }, () => null != SelectedSector || null != SelectedArea || null != SelectedRegion);

         RouteTappedCommand = new Command<Route>(async (route) => { await RouteTapped(route); });

         AddRouteCommand = new Command(async () => {
            var result = await GetService<IAlertService>().DisplayActionSheetAsync(
               Resources.Strings.Routes.Add_Title,
               Resources.Strings.Main.Cancel,
               null,
               Resources.Strings.Routes.Add_Region_Selection_Item,
               Resources.Strings.Routes.Add_Area_Selection_Item,
               Resources.Strings.Routes.Add_Sector_Selection_Item,
               Resources.Strings.Routes.Add_Route_From_Image_Selection_Item,
               Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item);

            if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Region_Selection_Item) == 0) {
               // show add region
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Area_Selection_Item) == 0) {
               // show add area
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Sector_Selection_Item) == 0) {
               // show add sector
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Image_Selection_Item) == 0) {
               // take picture and add route
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item) == 0) {
               // pick image and add route
            }
         });
      }

      protected virtual void InitializeRegions() {
         try {
            Regions = Client.RegionsClient.ListAsync().GetAwaiter().GetResult();
         } catch (RestApiCallException ex) {
            GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
         }

         // Selects first of the received regions
         if (Regions.Count > 0) {
            SelectedRegion = Regions[0];
         }
      }

      public virtual void OnSelectedRegionChanged() {
         Areas = null;
         SelectedArea = null;
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         if (null != SelectedRegion) {
            try {
               Areas = Client.AreasClient.ListAsync(SelectedRegion.Id?.ToString()).GetAwaiter().GetResult();
            } catch (RestApiCallException ex) {
               GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex).Wait();
               return;
            }

            // Selects first of the received areas
            if (Areas.Count > 0) {
               SelectedArea = Areas[0];
            }
         }
      }

      public virtual void OnSelectedAreaChanged() {
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         if (null != SelectedArea) {
            try {
               Sectors = Client.SectorsClient.ListAsync(SelectedArea.Id?.ToString()).GetAwaiter().GetResult();
            } catch (RestApiCallException ex) {
               GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex).Wait();
               return;
            }

            // Selects first of the received sectors
            if (Sectors.Count > 0) {
               SelectedSector = Sectors[0];
            }
         }
      }

      public virtual void OnSelectedSectorChanged() {
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         if (null != SelectedSector) {
            try {
               Routes = Client.RoutesClient.ListAsync(SelectedSector.Id?.ToString()).GetAwaiter().GetResult();
            } catch (RestApiCallException ex) {
               GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex).Wait();
            }
         }
      }

      public virtual void OnSelectedRouteChanged() {

      }

      private async Task RouteTapped(Route route) {
         try {
            await NavigationService.NavigateAsync(
               NavigationService.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
               route);
         } catch(System.Exception ex) {
            GetService<IErrorService>().LogException(ex);
         }
      }
   }
}