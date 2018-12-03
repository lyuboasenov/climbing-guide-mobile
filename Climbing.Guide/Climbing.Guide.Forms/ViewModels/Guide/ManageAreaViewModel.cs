using Climbing.Guide.Forms.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageAreaViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }

      public ObservableCollection<Climbing.Guide.Api.Schemas.Region> Regions { get; set; }
      public Climbing.Guide.Api.Schemas.Region SelectedRegion { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      public string Restrictions { get; set; }
      public string Access { get; set; }
      public string Descent { get; set; }

      public MapSpan Location { get; set; }

      public ManageAreaViewModel() {
         Title = VmTitle;
      }

      protected override void InitializeCommands() {
         SaveCommand = new Command(Save, CanSave);
         CancelCommand = new Command(async () => await GoBack());
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         var progressService = GetService<IProgressService>();
         try {
            await progressService.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
         } finally {
            await progressService.HideLoadingIndicatorAsync();
         }
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private bool CanSave() {
         return true;
      }

      private void Save() {

      }
   }
}