using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class GuideViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public ICommand ClearFilterCommand { get; private set; }
      public ICommand RouteTappedCommand { get; private set; }
      public ICommand AddRouteCommand { get; private set; }

      public GuideViewModel() {
         Title = VmTitle;
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
            var options = new List<string>() {
               Resources.Strings.Routes.Add_Region_Selection_Item,
               Resources.Strings.Routes.Add_Area_Selection_Item,
               Resources.Strings.Routes.Add_Sector_Selection_Item
            };

            var mediaService = GetService<IMediaService>();
            if (mediaService.IsTakePhotoSupported) {
               options.Add(Resources.Strings.Routes.Add_Route_From_Image_Selection_Item);
            }
            if (mediaService.IsPickPhotoSupported) {
               options.Add(Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item);
            }

            var result = await GetService<IAlertService>().DisplayActionSheetAsync(
               Resources.Strings.Routes.Add_Title,
               Resources.Strings.Main.Cancel,
               null,
               options.ToArray());

            if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Region_Selection_Item) == 0) {
               // show add region
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Area_Selection_Item) == 0) {
               // show add area
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Sector_Selection_Item) == 0) {
               // show add sector
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Image_Selection_Item) == 0) {
               // take picture and add route
               var navigationResult = await Navigation.NavigateAsync(
                  Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteEditView)),
                  SelectedRegion, SelectedArea, SelectedSector);
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item) == 0) {
               var path = await mediaService.PickPhotoAsync();
               if (System.IO.File.Exists(path)) {
                  // pick image and add route
                  var navigationResult = await Navigation.NavigateAsync(
                     Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteEditView)),
                     SelectedRegion, SelectedArea, SelectedSector, path);
               }
            }
         });
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         var progressService = GetService<IProgressService>();
         try {
            await progressService.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
         } finally {
            await progressService.HideLoadingIndicatorAsync();
         }
      }

      public override void OnSelectedRegionChanged() {
         base.OnSelectedRegionChanged();
         (ClearFilterCommand as Command).ChangeCanExecute();
      }

      public override void OnSelectedAreaChanged() {
         base.OnSelectedAreaChanged();
         (ClearFilterCommand as Command).ChangeCanExecute();
      }

      public override void OnSelectedSectorChanged() {
         base.OnSelectedSectorChanged();
         (ClearFilterCommand as Command).ChangeCanExecute();
      }

      private async Task RouteTapped(Route route) {
         var navigationResult = await Navigation.NavigateAsync(
            Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
            route);
         if (!navigationResult.Result) {
            await Errors.HandleExceptionAsync(navigationResult.Exception,
               Resources.Strings.Routes.Route_View_Error_Message, route.Name);
         }
      }
   }
}