using Alat.Validation;
using Alat.Validation.Rules;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.GeoLocation;
using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using INavigation = Climbing.Guide.Forms.Services.Navigation.INavigation;

namespace Climbing.Guide.Forms.ViewModels.Routes.Content {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageRouteViewModel : ParametrisedBaseViewModel<ManageRouteViewModel.Parameters>, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public static INavigationRequest GetNavigationRequest(INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.ManageRouteView), parameters);
      }

      public IValidationContext ValidationContext { get; }

      public ICommand ViewSchemaCommand { get; private set; }
      public ICommand SaveCommand { get; private set; }
      public ICommand CancelCommand { get; private set; }

      public string LocalSchemaThumbPath { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string RouteType { get; set; } = Resources.Strings.Routes.Route_Edit_Route_Type_Boulder;
      public IEnumerable<string> RouteTypes { get; set; }
      public double Length { get; set; }
      public string FA { get; set; }
      public ObservableCollection<Area> TraversalPath { get; set; }
      public Area ParentArea { get; set; }
      public ObservableCollection<Point> SchemaRoute { get; set; }
      public MapSpan Location { get; set; }

      private INavigation Navigation { get; }
      private IExceptionHandler ExceptionHandler { get; }
      private IGeoLocation GeoLocation { get; }

      public ManageRouteViewModel(IExceptionHandler exceptionHandler,
         INavigation navigation,
         IGeoLocation geoLocation,
         ValidationContextFactory validationContextFactory) {
         Navigation = navigation;
         ExceptionHandler = exceptionHandler;
         GeoLocation = geoLocation;

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();
         SchemaRoute = new ObservableCollection<Point>();

         InitializeCommands();

         ValidationContext = validationContextFactory.GetContextFor(this, true);
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await InitializeData(parameters);
      }

      public void OnValidationContextChanged() {
         // Raise validation context property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationContext));

         (SaveCommand as Command)?.ChangeCanExecute();
      }

      public void InitializeValidationRules(IValidationContext validationContext) {
         validationContext.AddRule<ManageRouteViewModel, string>(t => t.Name,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Name)));
         validationContext.AddRule<ManageRouteViewModel, string>(t => t.Info,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Info)));
         validationContext.AddRule<ManageRouteViewModel, MapSpan>(t => t.Location,
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
         if (parameters.TraversalPath == null) {
            throw new ArgumentNullException(nameof(parameters.TraversalPath));
         }

         if (string.IsNullOrEmpty(parameters.ImagePath)) {
            throw new ArgumentNullException(nameof(parameters.ImagePath));
         }

         try {
            await base.OnNavigatedToAsync(parameters);

            foreach (var area in parameters.TraversalPath) {
               TraversalPath.Add(area);
               ParentArea = area;
               if (area != null) {
                  Location = MapSpan.FromCenterAndRadius(
                     MapHelper.GetPosition(area.Latitude, area.Longitude),
                     Distance.FromKilometers(area.Size));
               }
            }

            LocalSchemaThumbPath = parameters.ImagePath;

            await InitializeLocationAsync();
         }catch(Exception ex) {
            await ExceptionHandler.HandleAsync(ex);
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
            double latitude, longitude;

            exifReader.GetTagValue(ExifLib.ExifTags.GPSLatitude, out double[] latitudeArray);
            exifReader.GetTagValue(ExifLib.ExifTags.GPSLongitude, out double[] longitudeArray);

            latitude = latitudeArray[0] + (latitudeArray[1] / 60) + (latitudeArray[2] / 3600);
            longitude = longitudeArray[0] + (longitudeArray[1] / 60) + (longitudeArray[2] / 3600);

            Location = MapSpan.FromCenterAndRadius(new Position(latitude, longitude), new Distance(170));
         }

         return Task.CompletedTask;
      }

      private async Task InitializeLocationWithCurrentAsync() {
         var location = await GeoLocation.GetCurrentOrDefaultAsync();
         Location = MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), new Distance(170));
      }

#pragma warning disable CA1822 // Mark members as static
      private Task ViewSchema() {
#pragma warning restore CA1822 // Mark members as static
         return Task.CompletedTask;
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
         public string ImagePath { get; set; }
      }
   }
}