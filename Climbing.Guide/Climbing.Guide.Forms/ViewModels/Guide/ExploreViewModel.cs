using Climbing.Guide.Forms.Helpers;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.GeoLocation;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ExploreViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Explore_Title;
      private static int[] ZoomLevel { get; } = new int[] { 5000000, 5000000, 22000, 1400, 170, 170, 170 };

      public ICommand PinTappedCommand { get; private set; }
      public ICommand TraverseBackCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }
      public IEnumerable Pins { get; set; }
      public MapSpan VisibleRegion { get; set; }
      public Position SelectedLocation { get; set; }

      private Progress Progress { get; }
      private GeoLocation GeoLocation { get; }

      public ExploreViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.Navigation navigation,
         Alerts alerts,
         Media media,
         Progress progress,
         GeoLocation geoLocation) : base(client, errors, media, alerts, navigation) {
         Progress = progress;
         GeoLocation = geoLocation;

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

            Pins = Items;

            decimal latitude, longitude;
            if (null == parentArea) {
               var location = await GeoLocation.GetCurrentOrDefaultAsync();
               latitude = (decimal)location.Latitude;
               longitude = (decimal)location.Longitude;
            } else {
               latitude = parentArea.Latitude;
               longitude = parentArea.Longitude;
            }

            int zoomLevel = ZoomLevel[TraversalPath.Count];
            VisibleRegion = MapSpan.FromCenterAndRadius(
               MapHelper.GetPosition(latitude, longitude),
               new Distance(zoomLevel));
         }
      }

      private void InitializeCommands() {
         PinTappedCommand = new Command(async (data) => { await OnPinTapped(data); } );
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalPath.Count > 1);
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private async Task OnPinTapped(object data) {
         if (data is Area) {
            await TraverseToAsync(data as Area);
         } else if (data is Route) {
            await ViewRoute(data as Route);
         }
      }

      private async Task OnTraverseBackAsync() {
         await TraverseBackAsync();
         (TraverseBackCommand as Command).ChangeCanExecute();
      }

      private Task OnAddAsync() {
         return AddItemAsync();
      }

      private async Task ViewRoute(Route route) {
         if (null != route) {
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
}