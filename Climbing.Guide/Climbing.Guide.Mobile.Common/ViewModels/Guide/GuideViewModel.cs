using Climbing.Guide.Core.API.Schemas;
using System;
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

      public ICommand ClearFilterCommand { get; }
      public ICommand RouteTappedCommand { get; }

      public GuideViewModel() {
         Title = VmTitle;

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

         // Initialization of regions
         Task.Run(UpdateFilter);
      }

      // Update can execute of the login command
      public void OnPropertyChanged(string propertyName, object before, object after) {
         if (propertyName.CompareTo(nameof(SelectedRegion)) == 0) {
            Areas = null;
            SelectedArea = null;
            Sectors = null;
            SelectedSector = null;
            Routes = null;
            SelectedRoute = null;
            Task.Run(UpdateFilter);
            (ClearFilterCommand as Command).ChangeCanExecute();
         }
         if (propertyName.CompareTo(nameof(SelectedArea)) == 0) {
            Sectors = null;
            SelectedSector = null;
            Routes = null;
            SelectedRoute = null;
            Task.Run(UpdateFilter);
            (ClearFilterCommand as Command).ChangeCanExecute();
         }
         if (propertyName.CompareTo(nameof(SelectedSector)) == 0) {
            Routes = null;
            SelectedRoute = null;
            Task.Run(UpdateFilter);
            (ClearFilterCommand as Command).ChangeCanExecute();
         }
         if (propertyName.CompareTo(nameof(SelectedRoute)) == 0) {
            RouteSelected();
         }

         RaisePropertyChanged(propertyName);
      }

      protected virtual async Task UpdateFilter() {
         if (null != SelectedSector) {
            // Load routes for the selected sector
            Routes = await Client.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
         } else if (null != SelectedArea) {
            Sectors = await Client.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            RaisePropertyChanged(nameof(Sectors));

            // Selects first of the received sectors
            if (Sectors.Count > 0) {
               SelectedSector = Sectors[0];
            }
         } else if (null != SelectedRegion) {
            Areas = await Client.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            RaisePropertyChanged(nameof(Areas));

            // Selects first of the received areas
            if (Areas.Count > 0) {
               SelectedArea = Areas[0];
            }
         } else {
            Regions = await Client.RegionsClient.ListAsync();
            RaisePropertyChanged(nameof(Regions));

            // Selects first of the received regions
            if (Regions.Count > 0) {
               SelectedRegion = Regions[0];
            }
         }
      }

      protected virtual void RouteSelected() {
         if (null != SelectedRoute) {
            Device.BeginInvokeOnMainThread(async () => {
               await Navigation.PushAsync<Routes.RouteViewModel>(SelectedRoute);
            });
         }
      }

      private async Task RouteTapped(Route route) {
         await Navigation.PushAsync<Routes.RouteViewModel>(route);
      }
   }
}