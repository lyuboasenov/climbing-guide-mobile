using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Common.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.Guide {
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

            Regions = null;
            Areas = null;
            Sectors = null;
            Routes = null;
         }, () => null != SelectedSector || null != SelectedArea || null != SelectedRegion);

         RouteTappedCommand = new Command<Route>(async (route) => { await RouteTapped(route); });

         AddRouteCommand = new Command(async () => {
            await GetService<IAlertService>().DisplayAlertAsync("Add route", "You are trying to add a route", Resources.Strings.Main.Ok);
         });
      }

      protected virtual void InitializeRegions() {
         Task.Run(async () => {
            try {
               Regions = await Client.RegionsClient.ListAsync();
            } catch (RestApiCallException ex) {
               await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
               return;
            }

            // Selects first of the received regions
            if (Regions.Count > 0) {
               SelectedRegion = Regions[0];
            }
         }).Wait();
      }

      public virtual void OnSelectedRegionChanged() {
         Areas = null;
         SelectedArea = null;
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         Task.Run(async () => {
            try {
               Areas = await Client.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            } catch (RestApiCallException ex) {
               await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
               return;
            }

            // Selects first of the received areas
            if (Areas.Count > 0) {
               SelectedArea = Areas[0];
            }
         }).Wait();
      }

      public virtual void OnSelectedAreaChanged() {
         Sectors = null;
         SelectedSector = null;
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         Task.Run(async () => {
            try {
               Sectors = await Client.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            } catch (RestApiCallException ex) {
               await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
               return;
            }

            // Selects first of the received sectors
            if (Sectors.Count > 0) {
               SelectedSector = Sectors[0];
            }
         }).Wait();
      }

      public virtual void OnSelectedSectorChanged() {
         Routes = null;
         SelectedRoute = null;
         (ClearFilterCommand as Command).ChangeCanExecute();

         Task.Run(async () => {
            try {
               Routes = await Client.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
            } catch (RestApiCallException ex) {
               await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
            }
         }).Wait();
      }

      public virtual void OnSelectedRouteChanged() {

      }

      private async Task RouteTapped(Route route) {
         await Navigation.PushAsync<Routes.RouteViewModel>(route);
      }
   }
}