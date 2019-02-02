using Alat.Validation;
using Alat.Validation.Rules;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.Alerts;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using INavigation = Climbing.Guide.Forms.Services.Navigation.INavigation;

namespace Climbing.Guide.Forms.ViewModels.Guide.Content.AddOrRemove {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class AreaViewModel : ParametrisedBaseViewModel<AreaViewModel.Parameters>, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public static INavigationRequest GetNavigationRequest(INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.AddOrEdit.AreaView), parameters);
      }

      public IValidationContext ValidationContext { get; }

      public string Name { get; set; }
      public string Info { get; set; }
      public string Restrictions { get; set; }
      public string Approach { get; set; }
      public string Descent { get; set; }

      public MapSpan Location { get; set; }

      public ObservableCollection<Area> TraversalPath { get; set; }
      public Area ParentArea { get; set; }

      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }

      private IApiClient Client { get; }
      private INavigation Navigation { get; }
      private IExceptionHandler ExceptionHandler { get; }
      private IAlerts Alerts { get; }

      public AreaViewModel(
         IApiClient client,
         INavigation navigation,
         IExceptionHandler exceptionHandler,
         IAlerts alerts,
         ValidationContextFactory validationContextFactory) {
         Client = client;
         Navigation = navigation;
         ExceptionHandler = exceptionHandler;
         Alerts = alerts;

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();

         InitializeCommands();

         ValidationContext = validationContextFactory.GetContextFor(this, true);
      }

      public void InitializeValidationRules(IValidationContext validationContext) {
         validationContext.AddRule<AreaViewModel, string>(t => t.Name,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Name)));
         validationContext.AddRule<AreaViewModel, string>(t => t.Info,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Info)));
         validationContext.AddRule<AreaViewModel, MapSpan>(t => t.Location,
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Map_Title)));
      }

      public void OnValidationContextChanged() {
         // Raise validation context property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationContext));

         (SaveCommand as Command)?.ChangeCanExecute();
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeData(parameters);
      }

      private void InitializeCommands() {
         SaveCommand = new Command(async () => await Save(), () => ValidationContext.IsValid);
         CancelCommand = new Command(async () => await GoBack());
      }

      private Task InitializeData(Parameters parameters) {
         try {
            if (null != parameters.TraversalPath) {
               foreach(var area in parameters.TraversalPath) {
                  TraversalPath.Add(area);
                  ParentArea = area;
                  if (null != area) {
                     Location = MapSpan.FromCenterAndRadius(
                        MapHelper.GetPosition(area.Latitude, area.Longitude),
                        Distance.FromKilometers(area.Size));
                  }
               }
            }
            return Task.CompletedTask;
         } catch (Exception ex) {
            return ExceptionHandler.HandleAsync(ex);
         }
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private async Task Save() {
         Area area = new Area() {
            Parent = ParentArea?.Id,

            Name = Name,
            Info = Info,

            Restrictions = Restrictions,
            Approach = Approach,
            Descent = Descent,

            Latitude = (decimal)Location.Center.Latitude,
            Longitude = (decimal)Location.Center.Longitude,
            Size = Location.Radius.Kilometers,
         };

         try {
            if (null != ParentArea) {
               await Client.AreasClient.CreateAsync(area, ParentArea.Id.Value);
            } else {
               await Client.AreasClient.CreateAsync(area);
            }
            await Alerts.DisplayAlertAsync(
               Resources.Strings.Guide.Manage_Area_Save_Successful_Title,
               Resources.Strings.Guide.Manage_Area_Save_Successful_Message,
               Resources.Strings.Main.Ok);
            await GoBack();
         } catch (Exception ex) {
            await ExceptionHandler.HandleAsync(ex, Resources.Strings.Guide.Manage_Area_Save_Error);
         }
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
      }
   }
}