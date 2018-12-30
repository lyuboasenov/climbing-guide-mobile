using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel : BaseViewModel {
      public ObservableCollection<object> Items { get; set; }

      private IApiClient Client { get; }
      protected IExceptionHandler Errors { get; }

      protected Stack<Area> TraversalPath { get; }

      public BaseGuideViewModel(
         IApiClient client,
         IExceptionHandler errors) {
         Client = client;
         Errors = errors;

         TraversalPath = new Stack<Area>();

         Items = new ObservableCollection<object>();
      }

      protected async virtual Task TraverseToAsync(Area parentArea) {
         TraversalPath.Push(parentArea);
         await LoadItemsAsync(parentArea);
      }

      protected async virtual Task TraverseBackAsync() {
         // Remove current parent
         TraversalPath.Pop();

         var parentArea = TraversalPath.Pop();
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
   }
}