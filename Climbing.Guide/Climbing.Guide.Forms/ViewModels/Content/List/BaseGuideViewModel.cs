using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Services.Alerts;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.Media;
using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Content.List {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseGuideViewModel<TParameters> : ParametrisedBaseViewModel<TParameters> where TParameters : class {
      public ObservableCollection<object> Items { get; set; }

      public  IApiClient Client { get; }

      public ObservableCollection<Area> TraversalPath { get; set; }

      protected Stack<Area> TraversalStack { get; }
      protected Area CurrentArea { get; private set; }

      protected INavigation Navigation { get; }
      protected IExceptionHandler ExceptionHandler { get; }

      private IMedia Media { get; }
      private IAlerts Alerts { get; }

      public BaseGuideViewModel(
         IApiClient client,
         IExceptionHandler exceptionHandler,
         IMedia media,
         IAlerts alerts,
         INavigation navigation) {
         Client = client;
         ExceptionHandler = exceptionHandler;
         Media = media;
         Alerts = alerts;
         Navigation = navigation;

         TraversalStack = new Stack<Area>();

         Items = new ObservableCollection<object>();
         TraversalPath = new ObservableCollection<Area>();
      }

      protected async virtual Task TraverseToAsync(Area area) {
         TraversalStack.Push(area);
         TraversalPath.Add(area);
         CurrentArea = area;
         await LoadItemsAsync(area);
      }

      protected async virtual Task TraverseBackAsync() {
         // Remove current parent
         TraversalStack.Pop();
         // Remove last two items
         TraversalPath.RemoveAt(TraversalPath.Count - 1);
         TraversalPath.RemoveAt(TraversalPath.Count - 1);

         var area = TraversalStack.Pop();
         await TraverseToAsync(area);
      }

      private async Task LoadItemsAsync(Area area) {
         Items.Clear();

         if (area?.Has_subareas ?? true) {
            await LoadAreasAsync(area);
         } else if (area?.Has_routes ?? false) {
            await LoadRoutesAsync(area);
         }
      }

      private async Task LoadAreasAsync(Area area) {
         await ExceptionHandler.ExecuteErrorHandled(async () => await LoadPagedAreasAsync(area));
      }

      private async Task LoadPagedAreasAsync(Area area) {
         bool hasMorePages = true;
         for (int page = 1; hasMorePages; page++) {
            IEnumerable<Area> areas = null;
            if (area == null) {
               var pagedAreas = await Client.AreasClient.ListAsync(page: page);
               areas = pagedAreas.Results;
               hasMorePages = pagedAreas.Next != null;
            } else {
               var pagedAreas = await Client.AreasClient.ListAsync(id: area.Id.Value, page: page);
               areas = pagedAreas.Results;
               hasMorePages = pagedAreas.Next != null;
            }

            foreach (var subArea in areas) {
               Items.Add(subArea);
            }
         }
      }

      private async Task LoadRoutesAsync(Area area) {
         if (area == null) {
            throw new ArgumentNullException(nameof(area));
         }

         try {
            await LoadPagedRoutesAsync(area);
         } catch (ApiCallException ex) {
            await ExceptionHandler.HandleAsync(ex);
         } catch (AggregateException ex) {
            await ExceptionHandler.HandleAsync(ex);
         }
      }

      private async Task LoadPagedRoutesAsync(Area area) {
         bool hasMorePages = true;
         for (int page = 1; hasMorePages; page++) {
            var pagedRoutes = await Client.RoutesClient.ListAsync(area.Id.Value, page);
            foreach (var route in pagedRoutes.Results) {
               Items.Add(route);
            }

            hasMorePages = pagedRoutes.Next != null;
         }
      }

      protected async Task AddItemAsync() {
         var options = GetAddItemAvailableOptions();

         var selectedOption = await Alerts.DisplayActionSheetAsync(
            Resources.Strings.Routes.Add_Title,
            Resources.Strings.Main.Cancel,
            null,
            (item) => item.Option,
            options.ToArray());

         await selectedOption?.ExecuteAsync();
      }

      private IEnumerable<AddItem> GetAddItemAvailableOptions() {
         var options = new List<AddItem>();

         if (!(CurrentArea?.Has_routes ?? false)) {
            options.Add(new AddArea(Navigation, TraversalPath));
         }

         if (!(CurrentArea?.Has_subareas ?? true)) {
            if (Media.IsTakePhotoSupported) {
               options.Add(new AddRouteFromCameraPhoto(Navigation, Media, TraversalPath));
            }
            if (Media.IsPickPhotoSupported) {
               options.Add(new AddRouteFromGalleryImage(Navigation, Media, TraversalPath));
            }
         }

         return options;
      }

      private abstract class AddItem {
         public abstract string Option { get; }

         protected INavigation Navigation { get; }
         protected ObservableCollection<Area> TraversalPath { get; }

         protected AddItem(INavigation navigation, ObservableCollection<Area> traversalPath) {
            Navigation = navigation;
            TraversalPath = traversalPath;
         }

         public abstract Task ExecuteAsync();
      }

      private class AddArea : AddItem {
         public override string Option => Resources.Strings.Routes.Add_Area_Selection_Item;

         public AddArea(INavigation navigation, ObservableCollection<Area> traversalPath) :
            base(navigation, traversalPath) { }

         public override async Task ExecuteAsync() {
            await Navigation.NavigateAsync(
               Guide.Content.ManageAreaViewModel.GetNavigationRequest(
                  Navigation,
                  new Guide.Content.ManageAreaViewModel.Parameters() {
                     TraversalPath = TraversalPath
                  }));
         }
      }

      private abstract class AddRoute : AddItem {
         protected IMedia Media { get; }

         protected AddRoute(INavigation navigation, IMedia media, ObservableCollection<Area> traversalPath) :
            base(navigation, traversalPath) {
            Media = media;
         }

         protected abstract Task<string> GetImagePath();

         public override async Task ExecuteAsync() {
            var path = await GetImagePath();
            if (System.IO.File.Exists(path)) {
               await Navigation.NavigateAsync(
                  Routes.Content.ManageRouteViewModel.GetNavigationRequest(
                     Navigation,
                     new Routes.Content.ManageRouteViewModel.Parameters() {
                        TraversalPath = TraversalPath,
                        ImagePath = path
                     }));
            }
         }
      }

      private class AddRouteFromGalleryImage : AddRoute {
         public override string Option => Resources.Strings.Routes.Add_Route_From_Gallery_Selection_Item;

         public AddRouteFromGalleryImage(INavigation navigation, IMedia media, ObservableCollection<Area> traversalPath) :
            base(navigation, media, traversalPath) { }

         protected override async Task<string> GetImagePath() {
            return await Media.PickPhotoAsync();
         }
      }

      private class AddRouteFromCameraPhoto : AddRoute {
         public override string Option => Resources.Strings.Routes.Add_Route_From_Image_Selection_Item;

         public AddRouteFromCameraPhoto(INavigation navigation, IMedia media, ObservableCollection<Area> traversalPath) :
            base(navigation, media, traversalPath) { }

         protected override async Task<string> GetImagePath() {
            return await Media.TakePhotoAsync();
         }
      }
   }
}