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

      public ICommand ClearFilterCommand { get; }

      public GuideViewModel() {
         Title = VmTitle;

         ClearFilterCommand = new Command(() => {
            SelectedSector = null;
            SelectedArea = null;
            SelectedRegion = null;

            Regions = null;
            Areas = null;
            Sectors = null;
            Routes = null;
         }, () => null != SelectedSector || null != SelectedArea || null != SelectedRegion);

         // Initialization of regions
         Task.Run(UpdateFilter);
      }

      // Update can execute of the login command
      public void OnPropertyChanged(string propertyName, object before, object after) {
         if (propertyName.CompareTo(nameof(SelectedRegion)) == 0 ||
            propertyName.CompareTo(nameof(SelectedArea)) == 0 ||
            propertyName.CompareTo(nameof(SelectedSector)) == 0) {
            Task.Run(UpdateFilter);
            (ClearFilterCommand as Command).ChangeCanExecute();
         }

         RaisePropertyChanged(propertyName);
      }

      protected virtual async Task UpdateFilter() {
         if (null != SelectedSector) {
            // Load routes for the selected sector
            Routes = await RestClient.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
         } else if (null != SelectedArea) {
            Sectors = await RestClient.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            RaisePropertyChanged(nameof(Sectors));
         } else if (null != SelectedRegion) {
            Areas = await RestClient.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            RaisePropertyChanged(nameof(Areas));
         } else {
            Regions = await RestClient.RegionsClient.ListAsync();
            RaisePropertyChanged(nameof(Regions));
         }
      }
   }
}