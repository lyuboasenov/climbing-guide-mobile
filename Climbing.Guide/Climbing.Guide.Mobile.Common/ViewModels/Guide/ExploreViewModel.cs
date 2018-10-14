using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Common.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;

namespace Climbing.Guide.Mobile.Common.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExploreViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      public ObservableCollection<Core.API.Schemas.Region> Regions { get; set; }
      public ObservableCollection<Area> Areas { get; set; }
      public ObservableCollection<Sector> Sectors { get; set; }
      public ObservableCollection<Route> Routes { get; set; }

      public Core.API.Schemas.Region SelectedRegion { get; set; }
      public Area SelectedArea { get; set; }
      public Sector SelectedSector { get; set; }
      public ObservableCollection<Pin> Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      public ICommand ClearFilterCommand { get; }

      public ExploreViewModel() {
         Title = VmTitle;

         ClearFilterCommand = new Command(() => {
            SelectedSector = null;
            SelectedArea = null;
            SelectedRegion = null;
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

         if(propertyName.CompareTo(nameof(VisibleRegion)) == 0) {
            Console.WriteLine($"Visible region: Center({VisibleRegion.Center.Latitude}, {VisibleRegion.Center.Longitude}); Radius({VisibleRegion.Radius.Meters}); LatitudeDegrees({VisibleRegion.LatitudeDegrees}); LongitudeDegrees({VisibleRegion.LongitudeDegrees});");
         }
         RaisePropertyChanged(propertyName);
      }

      private async Task UpdateFilter() {
         if (null != SelectedSector) {
            // Load routes for the selected sector
            Routes = await RestClient.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
            Pins = GetPins(Routes, (route) => MapUtils.GetPin(route.Name, route.Latitude, route.Longitude, data: route));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedSector.Latitude, SelectedSector.Longitude),
               new Distance(170));
         } else if (null != SelectedArea) {
            Sectors = await RestClient.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            Pins = GetPins(Sectors, (sector) => MapUtils.GetPin(sector.Name, sector.Latitude, sector.Longitude, data: sector));
            RaisePropertyChanged(nameof(Sectors));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedArea.Latitude, SelectedArea.Longitude),
               new Distance(1400));
         } else if (null != SelectedRegion) {
            Areas = await RestClient.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            Pins = GetPins(Areas, (area) => MapUtils.GetPin(area.Name, area.Latitude, area.Longitude, data: area));
            RaisePropertyChanged(nameof(Areas));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedRegion.Latitude, SelectedRegion.Longitude),
               new Distance(22000));
         } else {
            Regions = await RestClient.RegionsClient.ListAsync();
            Pins = GetPins(Regions, (region) => MapUtils.GetPin(region.Name, region.Latitude, region.Longitude, data: region));
            RaisePropertyChanged(nameof(Regions));

            var position = await Geolocation.GetLastKnownLocationAsync();

            VisibleRegion = MapSpan.FromCenterAndRadius(
               new Position(position.Latitude, position.Longitude),
               new Distance(5000000));

            RaisePropertyChanged(nameof(VisibleRegion));
         }
      }

      private ObservableCollection<Pin> GetPins<T>(IEnumerable<T> objects, Func<T, Pin> objectToPinConverter) {
         var result = new ObservableCollection<Pin>();
         foreach (T obj in objects) {
            result.Add(objectToPinConverter(obj));
         }
         return result;
      }
   }
}