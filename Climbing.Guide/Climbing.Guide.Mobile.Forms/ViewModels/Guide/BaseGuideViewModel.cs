using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Forms.Services;
using System.Collections.ObjectModel;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel : BaseViewModel {

      public ObservableCollection<Core.API.Schemas.Region> Regions { get; set; }
      public ObservableCollection<Area> Areas { get; set; }
      public ObservableCollection<Sector> Sectors { get; set; }
      public ObservableCollection<Route> Routes { get; set; }

      public Core.API.Schemas.Region SelectedRegion { get; set; }
      public Area SelectedArea { get; set; }
      public Sector SelectedSector { get; set; }
      public Route SelectedRoute { get; set; }

      public BaseGuideViewModel() {
         InitializeRegions();
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
   }
}