using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel : BaseViewModel {
      private IApiClient Client { get; }
      protected IExceptionHandler Errors { get; }
      private ISyncTaskRunner SyncTaskRunner { get; }

      protected Stack<Area> Breadcrumbs { get; private set; }

      public ObservableCollection<Area> Areas { get; set; }
      public ObservableCollection<Route> Routes { get; set; }

      public Area SelectedArea { get; set; }
      public Route SelectedRoute { get; set; }
      
      public BaseGuideViewModel(IApiClient client, IExceptionHandler errors, ISyncTaskRunner syncTaskRunner) {
         Client = client;
         Errors = errors;
         SyncTaskRunner = syncTaskRunner;

         Breadcrumbs = new Stack<Area>();

         Areas = new ObservableCollection<Area>();
         Routes = new ObservableCollection<Route>();
      }

      protected async override Task InitializeViewModel() {
         await InitializeAreasAsync(null);
      }

      protected async virtual Task InitializeAreasAsync(Area parentArea) {
         Areas.Clear();
         SelectedArea = null;
         Routes.Clear();
         SelectedRoute = null;

         try {
            for(int page = 1; ; page++) {
               bool hasMorePages = false;
               IEnumerable<Area> areas = null;
               if (null == parentArea) {
                  var pagedAreas = await Client.AreasClient.ListAsync(page: page);
                  areas = pagedAreas.Results;
                  hasMorePages = pagedAreas.Next != null;
               } else {
                  var pagedAreas = await Client.AreasClient.ListAsync(id: parentArea.Id.Value, page: page);
                  areas = pagedAreas.Results;
                  hasMorePages = pagedAreas.Next != null;
               }

               foreach (var area in areas) {
                  Areas.Add(area);
               }

               if (hasMorePages) {
                  break;
               }
            }
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex);
            return;
         }
      }

      protected async virtual Task InitializeRoutesAsync(Area parentArea) {

         if (null == parentArea) {
            throw new ArgumentNullException(nameof(parentArea));
         }

         Routes.Clear();
         SelectedRoute = null;

         try {
            for(int page = 1; ; page++) {
               var pagedRoutes = await Client.RoutesClient.ListAsync(parentArea.Id.Value, page);
               foreach (var route in pagedRoutes.Results) {
                  Routes.Add(route);
               }

               if (null == pagedRoutes.Next) {
                  break;
               }
            }
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex);
         }
      }

      public virtual void OnSelectedAreaChanged() {
         SyncTaskRunner.RunSync(async () => {
            var selectedArea = SelectedArea;
            Breadcrumbs.Push(selectedArea);

            if (selectedArea.Has_subareas??false) {
               await InitializeAreasAsync(selectedArea);
            } else if (selectedArea.Has_routes??false) {
               await InitializeRoutesAsync(selectedArea);
            }
         });
      }

      public virtual void OnSelectedRouteChanged() {
      }
   }
}