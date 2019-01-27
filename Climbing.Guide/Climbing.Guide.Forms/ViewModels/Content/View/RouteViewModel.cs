using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Services;
using Prism.Navigation;
using System;
using Climbing.Guide.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Climbing.Guide.Forms.Services.Navigation;

namespace Climbing.Guide.Forms.ViewModels.Content.View {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteViewModel : ParametrisedBaseViewModel<RouteViewModel.Parameters>, IDestructible {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public static INavigationRequest GetNavigationRequest(Services.Navigation.INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.View.RouteView), parameters);
      }

      private IApiClient Client { get; }
      private Services.IEnvironment Environment { get; }

      public Route Route { get; set; }
      public ICommand ViewSchemaCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }

      public RouteViewModel(IApiClient client, Services.IEnvironment environment) {
         Client = client;
         Environment = environment;

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

         Title = string.Format("{0}   {1}", Route.Name, Converters.GradeConverter.Convert(Route));

         var tempFile = Environment.GetTempFileName();
         await Client.DownloadAsync(Route.Schema, tempFile, true).ContinueWith((task) => {
            LocalSchemaThumbPath = tempFile;
         });

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