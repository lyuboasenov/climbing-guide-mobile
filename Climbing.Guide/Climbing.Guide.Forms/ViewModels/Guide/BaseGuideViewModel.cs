using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel : BaseViewModel {
      public ObservableCollection<object> Items { get; set; }

      public  IApiClient Client { get; }

      public ObservableCollection<Area> TraversalPath { get; set; }

      protected Stack<Area> TraversalStack { get; }

      protected INavigation Navigation { get; }
      protected IExceptionHandler Errors { get; }

      private IMedia Media { get; }
      private IAlerts Alerts { get; }

      public BaseGuideViewModel(
         IApiClient client,
         IExceptionHandler errors,
         IMedia media,
         IAlerts alerts,
         INavigation navigation) {
         Client = client;
         Errors = errors;
         Media = media;
         Alerts = alerts;
         Navigation = navigation;

         TraversalStack = new Stack<Area>();

         Items = new ObservableCollection<object>();
         TraversalPath = new ObservableCollection<Area>();
      }

      protected async virtual Task TraverseToAsync(Area parentArea) {
         TraversalStack.Push(parentArea);
         TraversalPath.Add(parentArea);
         await LoadItemsAsync(parentArea);
      }

      protected async virtual Task TraverseBackAsync() {
         // Remove current parent
         TraversalStack.Pop();
         // Remove last two items
         TraversalPath.RemoveAt(TraversalPath.Count - 1);
         TraversalPath.RemoveAt(TraversalPath.Count - 1);

         var parentArea = TraversalStack.Pop();
         await TraverseToAsync(parentArea);
      }

      private async Task LoadItemsAsync(Area parentArea) {
         Items.Clear();

         if (null == parentArea || (parentArea.Has_subareas??false)) {
            await LoadAreasAsync(parentArea);
         } else if (null != parentArea && (parentArea.Has_routes ?? false)) {
            await LoadRoutesAsync(parentArea);
         }
      }

      private async Task LoadAreasAsync(Area parentArea) {
         try {
            for(int page = 1; ; page++) {
               bool isLastPage = true;
               IEnumerable<Area> areas = null;
               if (null == parentArea) {
                  var pagedAreas = await Client.AreasClient.ListAsync(page: page);
                  areas = pagedAreas.Results;
                  isLastPage = pagedAreas.Next == null;
               } else {
                  var pagedAreas = await Client.AreasClient.ListAsync(id: parentArea.Id.Value, page: page);
                  areas = pagedAreas.Results;
                  isLastPage = pagedAreas.Next == null;
               }

               foreach (var area in areas) {
                  Items.Add(area);
               }

               if (isLastPage) {
                  break;
               }
            }
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex);
            return;
         } catch (AggregateException ex) {
            await Errors.HandleAsync(ex);
         }
      }

      private async Task LoadRoutesAsync(Area parentArea) {
         if (null == parentArea) {
            throw new ArgumentNullException(nameof(parentArea));
         }

         try {
            for(int page = 1; ; page++) {
               var pagedRoutes = await Client.RoutesClient.ListAsync(parentArea.Id.Value, page);
               foreach (var route in pagedRoutes.Results) {
                  Items.Add(route);
               }

               if (null == pagedRoutes.Next) {
                  break;
               }
            }
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex);
         } catch (AggregateException ex) {
            await Errors.HandleAsync(ex);
         }
      }

      protected async Task AddItemAsync() {
         var options = new List<string>() {
               Resources.Strings.Routes.Add_Area_Selection_Item
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
            await NavigateToManageArea();
         } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Image_Selection_Item) == 0) {
            var path = await Media.TakePhotoAsync();
            await NavigateToManageRoute(path);
         } else if (string.CompareOrdinal(result, Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item) == 0) {
            var path = await Media.PickPhotoAsync();
            if (System.IO.File.Exists(path)) {
               await NavigateToManageRoute(path);
            }
         }
      }

      private async Task NavigateToManageArea() {
         var navigationResult = await Navigation.NavigateAsync(
               Navigation.GetShellNavigationUri(nameof(Views.Guide.ManageAreaView)),
               TraversalPath);
      }

      private async Task NavigateToManageRoute(string imagePath) {
         var navigationResult = await Navigation.NavigateAsync(
               Navigation.GetShellNavigationUri(nameof(Views.Routes.ManageRouteView)),
               TraversalPath, imagePath);
      }
   }
}