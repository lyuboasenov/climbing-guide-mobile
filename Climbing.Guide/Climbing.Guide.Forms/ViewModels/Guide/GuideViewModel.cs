using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.Progress;
using System;
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
      private Services.INavigation Navigation { get; }
      private IProgress Progress { get; }
      public ICommand TraverseBackCommand { get; private set; }
      public ICommand ItemTappedCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }

      public GuideViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation,
         IAlerts alerts, 
         IMedia media,
         IProgress progress) :
         base(client, errors) {
         Alerts = alerts;
         Media = media;
         Navigation = navigation;
         Progress = progress;

         Title = VmTitle;

         InitializeCommands();
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         await TraverseToAsync(null);
      }

      protected async override Task TraverseToAsync(Area parentArea) {
         using (var loading = await Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(parentArea);
            (TraverseBackCommand as Command).ChangeCanExecute();
         }
      }

      private void InitializeCommands() {
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalPath.Count > 1);
         ItemTappedCommand = new Command<object>(async (item) => { await OnItemTappedAsync(item); });
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private async Task OnTraverseBackAsync() {
         await TraverseBackAsync();
         (TraverseBackCommand as Command).ChangeCanExecute();
      }

      private async Task OnAddAsync() {
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
               TraversalPath.Peek());
         } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item) == 0) {
            var path = await Media.PickPhotoAsync();
            if (System.IO.File.Exists(path)) {
               // pick image and add route
               var navigationResult = await Navigation.NavigateAsync(
                  Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteEditView)),
                  TraversalPath.Peek(), path);
            }
         }
      }

      private async Task OnItemTappedAsync(object item) {
         if (item is Area) {
            await TraverseToAsync(item as Area);
         } else if (item is Route) {
            await RouteTappedAsync(item as Route);
         } else {
            throw new ArgumentException(nameof(item));
         }
      }

      private async Task RouteTappedAsync(Route route) {
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