using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageRegionViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      public string Restrictions { get; set; }

      public MapSpan Location { get; set; }

      public ManageRegionViewModel() {
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