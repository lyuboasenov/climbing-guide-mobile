using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Queries;
using Climbing.Guide.Forms.Services.Environment;
using Climbing.Guide.Forms.Services.Navigation;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Content {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class DisplayRouteViewModel : ParametrisedBaseViewModel<DisplayRouteViewModel.Parameters>, IDestructible {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public static INavigationRequest GetNavigationRequest(Services.Navigation.INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.DisplayRouteView), parameters);
      }

      private IApiClient Client { get; }
      private IEnvironment Environment { get; }
      private IQueryFactory QueryFactory { get; }

      public Route Route { get; set; }
      public System.Windows.Input.ICommand ViewSchemaCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }

      public DisplayRouteViewModel(IApiClient client,
         IEnvironment environment,
         IQueryFactory commandQueryFactory) {
         Client = client ?? throw new ArgumentNullException(nameof(client));
         Environment = environment ?? throw new ArgumentNullException(nameof(environment));
         QueryFactory = commandQueryFactory ?? throw new ArgumentNullException(nameof(commandQueryFactory));

         Title = VmTitle;
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await base.OnNavigatedToAsync(parameters);
         await Initialize(parameters.Route);
      }

      private async Task Initialize(Route route) {
         if (route == null) {
            throw new ArgumentNullException(nameof(route));
         }
         var routeGrade = string.Empty;

         var query = QueryFactory.GetQuery<RouteGradeQuery>();
         try {
            routeGrade = (await query.GetResultAsync()).Name;
         } catch (KeyNotFoundException) {
            routeGrade = string.Empty;
         }

         Title = string.Format("{0}   {1}", Route.Name, routeGrade);

         var tempFile = Environment.GetTempFileName();
         await Client.DownloadAsync(Route.Schema, tempFile, true).ContinueWith((_) => LocalSchemaThumbPath = tempFile);

         SchemaRoute = new ObservableCollection<Point>() {
            new Point(0, 0), new Point(0.7, 0), new Point(0.7, 0.7), new Point(1, 1)
         };
      }

      private void CleanUp() {
         if (File.Exists(LocalSchemaThumbPath)) {
            File.Delete(LocalSchemaThumbPath);
         }
      }

      public void Destroy() {
         CleanUp();
      }

      public class Parameters {
         public Route Route { get; set; }
      }
   }
}