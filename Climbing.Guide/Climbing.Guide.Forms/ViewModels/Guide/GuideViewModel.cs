using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class GuideViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      private IAlerts Alerts { get; }
      private IMedia Media { get; }
      private IProgress Progress { get; }
      private Services.INavigation Navigation { get; }

      public ICommand ClearFilterCommand { get; private set; }
      public ICommand RouteTappedCommand { get; private set; }
      public ICommand AddRouteCommand { get; private set; }

      public GuideViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation,
         IAlerts alerts, 
         IMedia media, 
         IProgress progress, 
         ISyncTaskRunner syncTaskRunner) :
         base(client, errors, syncTaskRunner) {
         Alerts = alerts;
         Media = media;
         Progress = progress;
         Navigation = navigation;

         Title = VmTitle;

         InitializeCommands();
      }

      public override void OnSelectedAreaChanged() {
         base.OnSelectedAreaChanged();
         (ClearFilterCommand as Command).ChangeCanExecute();
      }

      private void InitializeCommands() {
         ClearFilterCommand = new Command(() => {
            SelectedRoute = null;
            SelectedArea = null;

            Areas = null;
            Routes = null;
         }, () => null != SelectedArea);

         RouteTappedCommand = new Command<Route>(async (route) => { await RouteTapped(route); });

         AddRouteCommand = new Command(async () => {
            var options = new List<string>() {
               Resources.Strings.Routes.Add_Area_Selection_Item,
               Resources.Strings.Routes.Add_Sector_Selection_Item
            };

            if (Media.IsTakePhotoSupported) {
               options.Add(Resources.Strings.Routes.Add_Route_From_Image_Selection_Item);
            }
            if (Media.IsPickPhotoSupported) {
               options.Add(Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item);
            }

            var result = await Alerts.DisplayActionSheetAsync(
               Resources.Strings.Routes.Add_Title,
               Resources.Strings.Main.Cancel,
               null,
               options.ToArray());

            if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Area_Selection_Item) == 0) {
               // show add area
               var navigationResult = await Navigation.NavigateAsync(
                  Navigation.GetShellNavigationUri(nameof(Views.Guide.ManageAreaView)));
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Sector_Selection_Item) == 0) {
               // show add sector
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Image_Selection_Item) == 0) {
               // take picture and add route
               var navigationResult = await Navigation.NavigateAsync(
                  Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteEditView)),
                  SelectedArea);
            } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item) == 0) {
               var path = await Media.PickPhotoAsync();
               if (System.IO.File.Exists(path)) {
                  // pick image and add route
                  var navigationResult = await Navigation.NavigateAsync(
                     Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteEditView)),
                     SelectedArea, path);
               }
            }
         });
      }

      private async Task RouteTapped(Route route) {
         var navigationResult = await Navigation.NavigateAsync(
            Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
            route);
         if (!navigationResult.Result) {
            await Errors.HandleAsync(navigationResult.Exception,
               Resources.Strings.Routes.Route_View_Error_Message, route.Name);
         }
      }
   }
}