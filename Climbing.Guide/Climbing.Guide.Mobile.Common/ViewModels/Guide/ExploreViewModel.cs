﻿using System;
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
   public class ExploreViewModel : GuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      public ObservableCollection<Pin> Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      public ExploreViewModel() {
         Title = VmTitle;
      }

      protected override async Task UpdateFilter() {
         if (null != SelectedSector) {
            // Load routes for the selected sector
            Routes = await Client.RoutesClient.ListAsync(SelectedSector.Id?.ToString());
            Pins = GetPins(Routes, (route) => MapUtils.GetPin(route.Name, route.Latitude, route.Longitude, data: route));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedSector.Latitude, SelectedSector.Longitude),
               new Distance(170));
         } else if (null != SelectedArea) {
            Sectors = await Client.SectorsClient.ListAsync(SelectedArea.Id?.ToString());
            Pins = GetPins(Sectors, (sector) => MapUtils.GetPin(sector.Name, sector.Latitude, sector.Longitude, data: sector));
            RaisePropertyChanged(nameof(Sectors));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedArea.Latitude, SelectedArea.Longitude),
               new Distance(1400));
         } else if (null != SelectedRegion) {
            Areas = await Client.AreasClient.ListAsync(SelectedRegion.Id?.ToString());
            Pins = GetPins(Areas, (area) => MapUtils.GetPin(area.Name, area.Latitude, area.Longitude, data: area));
            RaisePropertyChanged(nameof(Areas));
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapUtils.GetPosition(SelectedRegion.Latitude, SelectedRegion.Longitude),
               new Distance(22000));
         } else {
            Regions = await Client.RegionsClient.ListAsync();
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