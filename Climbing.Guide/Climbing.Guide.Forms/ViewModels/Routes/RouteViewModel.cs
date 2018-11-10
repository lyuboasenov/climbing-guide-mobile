using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Tasks;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Routes {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteViewModel : BaseViewModel, IDisposable {
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
            GetService<ITaskRunner>().Run(() => Initialize(Route));
         }
      }

      private async Task Initialize(Route route) {
         if (route == null) {
            throw new ArgumentNullException(nameof(route));
         }

         Title = string.Format("{0}   {1}", Route.Name, Converters.GradeConverter.Convert(Route));

         var tempFile = GetService<IEnvironment>().GetTempFileName();
         await Client.DownloadAsync(Route.Schema, tempFile, true).ContinueWith((task) => {
            LocalSchemaThumbPath = tempFile;
         });

         SchemaRoute = new ObservableCollection<Point>() {
            new Point(0, 0), new Point(0.7, 0), new Point(0.7, 0.7), new Point(1, 1)
         };
      }
      
      private async Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
      }

      // Implement IDisposable.
      // Do not make this method virtual.
      // A derived class should not be able to override this method.
      public void Dispose() {
         Dispose(true);
         // This object will be cleaned up by the Dispose method.
         // Therefore, you should call GC.SupressFinalize to
         // take this object off the finalization queue
         // and prevent finalization code for this object
         // from executing a second time.
         GC.SuppressFinalize(this);
      }

      // Dispose(bool disposing) executes in two distinct scenarios.
      // If disposing equals true, the method has been called directly
      // or indirectly by a user's code. Managed and unmanaged resources
      // can be disposed.
      // If disposing equals false, the method has been called by the
      // runtime from inside the finalizer and you should not reference
      // other objects. Only unmanaged resources can be disposed.
      protected virtual void Dispose(bool disposing) {
         if (File.Exists(LocalSchemaThumbPath)) {
            File.Delete(LocalSchemaThumbPath);
         }
      }
   }
}