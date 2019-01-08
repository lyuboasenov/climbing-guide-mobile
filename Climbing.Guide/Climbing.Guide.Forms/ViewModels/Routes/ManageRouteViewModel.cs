using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using System;
using System.Collections.Generic;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Climbing.Guide.Forms.Helpers;

namespace Climbing.Guide.Forms.ViewModels.Routes {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageRouteViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      private Services.INavigation Navigation { get; }
      private IExceptionHandler Errors { get; }

      public ICommand ViewSchemaCommand { get; set; }
      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      // TODO: fix grades
      // public IGrade SelectedDifficulty { get; set; }
      // public IEnumerable<IGrade> Difficulty { get; set; } = GetService<IGradeService>().GetGradeList(Core.Models.Routes.GradeType.V);
      public string RouteType { get; set; } = Resources.Strings.Routes.Route_Edit_Route_Type_Boulder;
      public IEnumerable<string> RouteTypes { get; set; }
      public double Length { get; set; }
      public string FA { get; set; }

      public ObservableCollection<Area> TraversalPath { get; set; }
      public Area ParentArea { get; set; }

      public ObservableCollection<Point> SchemaRoute { get; set; }

      public MapSpan Location { get; set; }

      public ManageRouteViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation) {
         Navigation = navigation;
         Errors = errors;

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();
         SchemaRoute = new ObservableCollection<Point>();

         InitializeCommands();
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await InitializeData(parameters);
      }

      private void InitializeCommands() {
         ViewSchemaCommand = new Command(async () => await ViewSchema());
         SaveCommand = new Command(Save, CanSave);
         CancelCommand = new Command(async () => await GoBack());
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync(TraversalPath);
      }

      private bool CanSave() {
         return true;
      }

      private void Save() {
         
      }

      private async Task InitializeData(params object[] parameters) {
         try {
            await base.OnNavigatedToAsync(parameters);
            var traversalPath = parameters != null && parameters.Length > 0 ? parameters[0] as IEnumerable<Area> : null;
            var imagePath = parameters != null && parameters.Length > 1 ? parameters[1] as string : null;

            if (null == traversalPath) {
               new ArgumentNullException(nameof(traversalPath));
            } else {
               foreach (var area in traversalPath) {
                  TraversalPath.Add(area);
                  ParentArea = area;
                  if (null != area) {
                     Location = MapSpan.FromCenterAndRadius(
                        MapHelper.GetPosition(area.Latitude, area.Longitude),
                        Distance.FromKilometers(area.Size));
                  }
               }
            }

            if (string.IsNullOrEmpty(imagePath)) {
               new ArgumentNullException(nameof(imagePath));
            } else {
               LocalSchemaThumbPath = imagePath;
            }

            LocalSchemaThumbPath = parameters[1] as string;

            using (var exifReader = new ExifLib.ExifReader(LocalSchemaThumbPath)) {
               double[] latitudeArray, longitudeArray;
               double latitude, longitude;

               exifReader.GetTagValue(ExifLib.ExifTags.GPSLatitude, out latitudeArray);
               exifReader.GetTagValue(ExifLib.ExifTags.GPSLongitude, out longitudeArray);

               latitude = latitudeArray[0] + latitudeArray[1] / 60 + latitudeArray[2] / 3600;
               longitude = longitudeArray[0] + longitudeArray[1] / 60 + longitudeArray[2] / 3600;

               Location = MapSpan.FromCenterAndRadius(new Position(latitude, longitude), new Distance(170));
            }
         }catch(Exception ex) {
            await Errors.HandleAsync(ex);
         }
      }

      private Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
         return Task.CompletedTask;
      }
   }
}