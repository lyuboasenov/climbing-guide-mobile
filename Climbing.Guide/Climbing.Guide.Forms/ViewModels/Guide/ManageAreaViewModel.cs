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

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageAreaViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public IDictionary<string, IEnumerable<string>> ValidationErrors => new Dictionary<string, IEnumerable<string>>();
      public IDictionary<string, IEnumerable<IRule>> ValidationRules => new Dictionary<string, IEnumerable<IRule>>();
      public bool IsValid { get; set; }

      public ObservableCollection<Area> Areas { get; set; }
      public Area SelectedArea { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      public string Restrictions { get; set; }
      public string Access { get; set; }
      public string Descent { get; set; }

      public MapSpan Location { get; set; }

      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }

      private Services.INavigation Navigation { get; }
      private IValidator Validator { get; }

      public ManageAreaViewModel(
         Services.INavigation navigation,
         IValidator validator) {
         Navigation = navigation;
         Validator = validator;

         Title = VmTitle;

         Areas = new ObservableCollection<Area>();

         InitializeValidationRules();
         InitializeCommands();
      }

      private void InitializeCommands() {
         SaveCommand = new Command(Save, () => IsValid);
         CancelCommand = new Command(async () => await GoBack());
      }

      private void InitializeValidationRules() {
         this.AddRule(nameof(Name),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Name)));
         this.AddRule(nameof(Info),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Info)));
         this.AddRule(nameof(Location),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Map_Title)));
         this.AddRule(nameof(SelectedArea),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Region)));
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         Validator.Validate(this, propertyName, after);
         // Raise validation errors property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationErrors));

         (SaveCommand as Command).ChangeCanExecute();
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private void Save() {

      }
   }
}