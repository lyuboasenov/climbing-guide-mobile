using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Tasks;
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
         Regions = new ObservableCollection<Region>();
         Areas = new ObservableCollection<Area>();
         Sectors = new ObservableCollection<Sector>();
         Routes = new ObservableCollection<Route>();
      }

      protected async override Task InitializeViewModel() {
         await InitializeRegionsAsync();
      }

      protected async virtual Task InitializeRegionsAsync() {
         try {
            var regions = await GetService<Services.IResourceService>().GetRegionsAsync();
            foreach(var region in regions) {
               Regions.Add(region);
            }
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex);
         }

         // Selects first of the received regions
         if (AutoSelectRegions && Regions.Count > 0) {
            SelectedRegion = Regions[0];
         }
      }

      protected async virtual Task InitializeAreasAsync() {
         Areas.Clear();
         SelectedArea = null;
         Sectors.Clear();
         SelectedSector = null;
         Routes.Clear();
         SelectedRoute = null;

         if (null != SelectedRegion) {
            try {
               var areas = await Client.AreasClient.ListAsync(SelectedRegion.Id.Value);
               foreach(var area in areas.Results) {
                  Areas.Add(area);
               }
            } catch (ApiCallException ex) {
               await Errors.HandleAsync(ex);
               return;
            }

            // Selects first of the received areas
            if (AutoSelectAreas && Areas.Count > 0) {
               SelectedArea = Areas[0];
            }
         }
      }

      protected async virtual Task InitializeSectorsAsync() {
         Sectors.Clear();
         SelectedSector = null;
         Routes.Clear();
         SelectedRoute = null;

         if (null != SelectedArea) {
            try {
               var sectors = await Client.SectorsClient.ListAsync(SelectedArea.Id.Value);
               foreach(var sector in sectors.Results) {
                  Sectors.Add(sector);
               }
            } catch (ApiCallException ex) {
               await Errors.HandleAsync(ex);
               return;
            }

            // Selects first of the received sectors
            if (AutoSelectSectors && Sectors.Count > 0) {
               SelectedSector = Sectors[0];
            }
         }
      }

      protected async virtual Task InitializeRoutesAsync() {
         Routes.Clear();
         SelectedRoute = null;

         if (null != SelectedSector) {
            try {
               var routes = (await Client.RoutesClient.ListAsync(SelectedSector.Id.Value)).Results;
               foreach(var route in routes) {
                  Routes.Add(route);
               }
            } catch (ApiCallException ex) {
               await Errors.HandleAsync(ex);
            }

            if (AutoSelectRoutes && Routes.Count > 0) {
               SelectedRoute = Routes[0];
            }
         }
      }

      public virtual void OnSelectedRegionChanged() {
         GetService<ISyncTaskRunner>().RunSync(async () => { await InitializeAreasAsync(); });
      }

      public virtual void OnSelectedAreaChanged() {
         GetService<ISyncTaskRunner>().RunSync(async () => { await InitializeSectorsAsync(); });
      }

      public virtual void OnSelectedSectorChanged() {
         GetService<ISyncTaskRunner>().RunSync(async () => { await InitializeRoutesAsync(); });
      }

      public virtual void OnSelectedRouteChanged() {
      }
   }
}