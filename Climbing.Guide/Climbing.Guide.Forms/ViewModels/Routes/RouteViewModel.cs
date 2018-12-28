using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Tasks;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Routes {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteViewModel : BaseViewModel, IDestructible {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      private IApiClient Client { get; }
      private IEnvironment Environment { get; }

      public Route Route { get; set; }
      public ICommand ViewSchemaCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }

      public RouteViewModel(IApiClient client, IEnvironment environment) {
         Client = client;
         Environment = environment;

         Title = VmTitle;
      }
      
      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         Route = parameters[0] as Route;
         await Initialize(Route);
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
   }
}