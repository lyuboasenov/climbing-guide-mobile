using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Climbing.Guide.Mobile.Forms.Helpers;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Climbing.Guide.Mobile.Forms.Services;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExploreViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;

      public ObservableCollection<Pin> Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      public ExploreViewModel() {
         Title = VmTitle;
      }

      protected override void InitializeRegions() {
         base.InitializeRegions();

         Pins = GetPins(Regions, (region) => MapHelper.GetPin(region.Name, region.Latitude, region.Longitude, data: region));
         Location position = null;
         try {
            position = Geolocation.GetLastKnownLocationAsync().GetAwaiter().GetResult();
         } catch (PermissionException pEx) {
            Task.Run(async () => 
               await GetService<IErrorService>().HandleExceptionAsync(pEx,
                  Resources.Strings.Main.Permission_Exception_Format,
                  Resources.Strings.Main.Location_Permissino)
            );
         }
         
         if (null != position) {
            VisibleRegion = MapSpan.FromCenterAndRadius(
            new Position(position.Latitude, position.Longitude),
            new Distance(5000000));
         }
      }

      public override void OnSelectedRegionChanged() {
         base.OnSelectedRegionChanged();

         Pins = GetPins(Areas, (area) => MapHelper.GetPin(area.Name, area.Latitude, area.Longitude, data: area));
         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedRegion.Latitude, SelectedRegion.Longitude),
            new Distance(22000));
      }

      public override void OnSelectedAreaChanged() {
         base.OnSelectedAreaChanged();

         Pins = GetPins(Sectors, (sector) => MapHelper.GetPin(sector.Name, sector.Latitude, sector.Longitude, data: sector));
         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedArea.Latitude, SelectedArea.Longitude),
            new Distance(1400));
      }

      public override void OnSelectedSectorChanged() {
         base.OnSelectedSectorChanged();

         Pins = GetPins(Routes, (route) => MapHelper.GetPin(route.Name, route.Latitude, route.Longitude, data: route));
         VisibleRegion = MapSpan.FromCenterAndRadius(
            MapHelper.GetPosition(SelectedSector.Latitude, SelectedSector.Longitude),
            new Distance(170));
      }

      public override void OnSelectedRouteChanged() {
         base.OnSelectedRouteChanged();
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