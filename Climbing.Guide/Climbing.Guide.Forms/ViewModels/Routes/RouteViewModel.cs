using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Tasks;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Routes {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public Route Route { get; set; }
      public ICommand ViewSchemaCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }

      public RouteViewModel() {
         Title = VmTitle;

         ViewSchemaCommand = new Command(async () => await ViewSchema());
      }

      public override void OnNavigatedTo(params object[] parameters) {
         base.OnNavigatedTo(parameters);
         Route = parameters[0] as Route;
         if (null != Route) {
            Title = string.Format("{0}   {1}", Route.Name, Converters.GradeConverter.Convert(Route));

            GetService<ITaskRunner>().Run(() => Client.DownloadRouteSchemaAsync(Route.Id.Value, Route.Schema))
               .ContinueWith((task) => {
                  LocalSchemaThumbPath = task.Result;
               });

            SchemaRoute = new ObservableCollection<Point>() {
            new Point(0, 0), new Point(0.7, 0), new Point(0.7, 0.7), new Point(1, 1)
         };
         }
      }

      private async Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
      }
   }
}