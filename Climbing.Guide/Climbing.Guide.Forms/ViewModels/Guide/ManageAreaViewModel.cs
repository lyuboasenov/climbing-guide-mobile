using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Validations;
using Climbing.Guide.Forms.Validations.Rules;
using System.Collections.Generic;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Helpers;
using Climbing.Guide.Core.Api;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageAreaViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public IDictionary<string, IEnumerable<string>> ValidationErrors { get; } = new Dictionary<string, IEnumerable<string>>();
      public IDictionary<string, IEnumerable<IRule>> ValidationRules { get; } = new Dictionary<string, IEnumerable<IRule>>();
      public bool IsValid { get; set; }

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
      private Services.INavigation Navigation { get; }
      private IValidator Validator { get; }
      private IExceptionHandler Errors { get; }
      private IAlerts Alerts { get; }
      private bool IsInitialized { get; } = false;

      public ManageAreaViewModel(
         IApiClient client,
         Services.INavigation navigation,
         IValidator validator,
         IExceptionHandler exceptionHandler,
         IAlerts alerts) {
         Client = client;
         Navigation = navigation;
         Validator = validator;
         Errors = exceptionHandler;
         Alerts = alerts;

         Title = VmTitle;

         TraversalPath = new ObservableCollection<Area>();

         InitializeValidationRules();
         InitializeCommands();

         IsInitialized = true;
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeData(parameters);
      }

      private void InitializeCommands() {
         SaveCommand = new Command(async () => await Save(), () => IsValid);
         CancelCommand = new Command(async () => await GoBack());
      }

      private void InitializeValidationRules() {
         this.AddRule(nameof(Name),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Name)));
         this.AddRule(nameof(Info),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Info)));
         this.AddRule(nameof(Location),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Map_Title)));
      }

      private Task InitializeData(params object[] parameters) {
         try {
            var traversalPath = parameters != null && parameters.Length > 0 ? parameters[0] as IEnumerable<Area> : null;
            if (null != traversalPath) {
               foreach(var area in traversalPath) {
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
            return Errors.HandleAsync(ex);
         }
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         if (IsInitialized) {
            Validator.Validate(this, propertyName, after);
            // Raise validation errors property changed in order to update validation errors
            RaisePropertyChanged(nameof(ValidationErrors));

            (SaveCommand as Command).ChangeCanExecute();
         }
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync(TraversalPath);
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
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex, Resources.Strings.Guide.Manage_Area_Save_Error);
         } catch (AggregateException ex) {
            await Errors.HandleAsync(ex, Resources.Strings.Guide.Manage_Area_Save_Error);
         } catch (Exception ex) {
            await Errors.HandleAsync(ex, Resources.Strings.Guide.Manage_Area_Save_Error);
         }
      }
   }
}