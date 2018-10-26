using Climbing.Guide.Core.API.Schemas;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.Routes {
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

      //TODO
      //public override void Init(object initData) {
      //   Route = initData as Route;
      //   Title = string.Format("{0}   {1}", Route.Name, Converters.GradeConverter.Convert(Route));
         
      //   Task.Run(() => Client.DownloadRouteSchemaAsync(Route.Id.Value, Route.Schema)).ContinueWith((task) => {
      //      LocalSchemaThumbPath = task.Result;
      //   });

      //   SchemaRoute = new ObservableCollection<Point>() {
      //      new Point(0, 0), new Point(0.7, 0), new Point(0.7, 0.7), new Point(1, 1)
      //   };
      //}

      private async Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
      }
   }
}