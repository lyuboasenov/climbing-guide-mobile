using Climbing.Guide.Api.Schemas;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel : BaseViewModel {

      public ObservableCollection<Region> Regions { get; set; }
      public ObservableCollection<Area> Areas { get; set; }
      public ObservableCollection<Sector> Sectors { get; set; }
      public ObservableCollection<Route> Routes { get; set; }

      public Region SelectedRegion { get; set; }
      public Area SelectedArea { get; set; }
      public Sector SelectedSector { get; set; }
      public Route SelectedRoute { get; set; }

      public virtual bool AutoSelectRegions { get; } = true;
      public virtual bool AutoSelectAreas { get; } = true;
      public virtual bool AutoSelectSectors { get; } = true;
      public virtual bool AutoSelectRoutes { get; } = false;

      public BaseGuideViewModel() {
         
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeRegionsAsync();
      }

      protected async virtual Task InitializeRegionsAsync() {
         try {
            Regions = await Client.RegionsClient.ListAsync();
         } catch (ApiCallException ex) {
            await Errors.HandleApiCallExceptionAsync(ex);
         }

         // Selects first of the received regions
         if (AutoSelectRegions && Regions.Count > 0) {
            SelectedRegion = Regions[0];
         }
      }

      protected async virtual Task InitializeAreasAsync() {
         Areas = null;
         SelectedArea = null;
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;

         if (null != SelectedRegion) {
            try {
               Areas = await Client.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            } catch (ApiCallException ex) {
               await Errors.HandleApiCallExceptionAsync(ex);
               return;
            }

            // Selects first of the received areas
            if (AutoSelectAreas && Areas.Count > 0) {
               SelectedArea = Areas[0];
            }
         }
      }

      protected async virtual Task InitializeSectorsAsync() {
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;

         if (null != SelectedArea) {
            try {
               Sectors = await Client.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            } catch (ApiCallException ex) {
               await Errors.HandleApiCallExceptionAsync(ex);
               return;
            }

            // Selects first of the received sectors
            if (AutoSelectSectors && Sectors.Count > 0) {
               SelectedSector = Sectors[0];
            }
         }
      }

      protected async virtual Task InitializeRoutesAsync() {
         Routes = null;
         SelectedRoute = null;

         if (null != SelectedSector) {
            try {
               Routes = await Client.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
            } catch (ApiCallException ex) {
               await Errors.HandleApiCallExceptionAsync(ex);
            }

            if (AutoSelectRoutes && Routes.Count > 0) {
               SelectedRoute = Routes[0];
            }
         }
      }

      public async virtual void OnSelectedRegionChanged() {
         await InitializeAreasAsync();
      }

      public async virtual void OnSelectedAreaChanged() {
         await InitializeSectorsAsync();
      }

      public async virtual void OnSelectedSectorChanged() {
         await InitializeRoutesAsync();
      }

      public virtual void OnSelectedRouteChanged() {
      }
   }
}