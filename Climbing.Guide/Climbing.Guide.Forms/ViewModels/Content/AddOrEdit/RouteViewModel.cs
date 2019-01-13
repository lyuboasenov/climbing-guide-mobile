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
using Climbing.Guide.Forms.Services.GeoLocation;
using Climbing.Guide.Forms.Services.Navigation;
using Alat.Validation;
using Alat.Validation.Rules;

namespace Climbing.Guide.Forms.ViewModels.Routes.Content.AddOrRemove {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteViewModel : ParametrisedBaseViewModel<RouteViewModel.Parameters>, Validatable {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public static NavigationRequest GetNavigationRequest(Navigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.AddOrEdit.RouteView), parameters);
      }

      public ValidationContext ValidationContext { get; }

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

      private Navigation Navigation { get; }
      private IExceptionHandler Errors { get; }
      private GeoLocation GeoLocation { get; }

      public RouteViewModel(IApiClient client,
         IExceptionHandler errors,
         Navigation navigation,
         GeoLocation geoLocation,
         ValidationContextFactory validationContextFactory) {
         Navigation = navigation;
         Errors = errors;
         GeoLocation = geoLocation;

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();
         SchemaRoute = new ObservableCollection<Point>();

         InitializeCommands();

         // ValidationContext should be initialized after all other initialization is done
         ValidationContext = validationContextFactory.GetContextFor(this, true);
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await InitializeData(parameters);
      }

      public void OnValidationContextChanged() {
         // Raise validation context property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationContext));

         (SaveCommand as Command).ChangeCanExecute();
      }

      public void InitializeValidationRules(ValidationContext context) {
         context.AddRule<RouteViewModel, string>(t => t.Name,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Name)));
         context.AddRule<RouteViewModel, string>(t => t.Info,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Info)));
         context.AddRule<RouteViewModel, MapSpan>(t => t.Location,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Map_Title)));
      }

      private void InitializeCommands() {
         ViewSchemaCommand = new Command(async () => await ViewSchema());
         SaveCommand = new Command(async () => await Save(), () => ValidationContext.IsValid);
         CancelCommand = new Command(async () => await GoBack());
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private async Task Save() {
         await Navigation.GoBackAsync();
      }

      private async Task InitializeData(Parameters parameters) {
         try {
            await base.OnNavigatedToAsync(parameters);

            if (null == parameters.TraversalPath) {
               new ArgumentNullException(nameof(parameters.TraversalPath));
            } else {
               foreach (var area in parameters.TraversalPath) {
                  TraversalPath.Add(area);
                  ParentArea = area;
                  if (null != area) {
                     Location = MapSpan.FromCenterAndRadius(
                        MapHelper.GetPosition(area.Latitude, area.Longitude),
                        Distance.FromKilometers(area.Size));
                  }
               }
            }

            if (string.IsNullOrEmpty(parameters.ImagePath)) {
               new ArgumentNullException(nameof(parameters.ImagePath));
            } else {
               LocalSchemaThumbPath = parameters.ImagePath;
            }

            await InitializeLocationAsync();
         }catch(Exception ex) {
            await Errors.HandleAsync(ex);
         }
      }

      private async Task InitializeLocationAsync() {
         try {
            await InitializeLocationFromExifAsync();
         } catch {
            await InitializeLocationWithCurrentAsync();
         }
      }

      private Task InitializeLocationFromExifAsync() {
         using (var exifReader = new ExifLib.ExifReader(LocalSchemaThumbPath)) {
            double[] latitudeArray, longitudeArray;
            double latitude, longitude;

            exifReader.GetTagValue(ExifLib.ExifTags.GPSLatitude, out latitudeArray);
            exifReader.GetTagValue(ExifLib.ExifTags.GPSLongitude, out longitudeArray);

            latitude = latitudeArray[0] + latitudeArray[1] / 60 + latitudeArray[2] / 3600;
            longitude = longitudeArray[0] + longitudeArray[1] / 60 + longitudeArray[2] / 3600;

            Location = MapSpan.FromCenterAndRadius(new Position(latitude, longitude), new Distance(170));
         }

         return Task.CompletedTask;
      }

      private async Task InitializeLocationWithCurrentAsync() {
         var location = await GeoLocation.GetCurrentOrDefaultAsync();
         Location = MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), new Distance(170));
      }

      private Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
         return Task.CompletedTask;
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
         public string ImagePath { get; set; }
      }
   }
}