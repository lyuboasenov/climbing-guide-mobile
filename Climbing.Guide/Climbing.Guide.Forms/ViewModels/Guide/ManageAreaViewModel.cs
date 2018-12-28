using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Validations.Rules;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageAreaViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      private Services.INavigation Navigation { get; }
      private IProgress Progress { get; }

      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }
      public ObservableCollection<Area> Areas { get; set; }
      public Area SelectedArea { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      public string Restrictions { get; set; }
      public string Access { get; set; }
      public string Descent { get; set; }

      public MapSpan Location { get; set; }

      public ManageAreaViewModel(Services.INavigation navigation, IProgress progress) {
         Progress = progress;
         Navigation = navigation;

         Title = VmTitle;

         Areas = new ObservableCollection<Area>();
      }

      protected override void InitializeCommands() {
         SaveCommand = new Command(Save, CanSave);
         CancelCommand = new Command(async () => await GoBack());
      }

      protected override void InitializeValidationRules() {
         base.InitializeValidationRules();
         AddValidationRule(nameof(Name),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Name)));
         AddValidationRule(nameof(Info),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Info)));
         AddValidationRule(nameof(Location),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Region_Map_Title)));
         AddValidationRule(nameof(SelectedArea),
            new RequiredRule(
               string.Format(
                  Resources.Strings.Main.Validation_Required_Field,
                  Resources.Strings.Guide.Manage_Area_Region)));
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         try {
            await Progress.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
         } finally {
            await Progress.HideLoadingIndicatorAsync();
         }
      }

      public override void OnPropertyChanged(string propertyName, object before, object after) {
         base.OnPropertyChanged(propertyName, before, after);

         var saveCommand = SaveCommand as Command;
         if (null != saveCommand) {
            saveCommand.ChangeCanExecute();
         }
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private bool CanSave() {
         return !HasValidationErrors;
      }

      private void Save() {

      }
   }
}