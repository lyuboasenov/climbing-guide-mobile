using Climbing.Guide.Api.Schemas;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Settings {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SettingsViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Settings.Settings_Title;

      public Language SelectedLanguage { get; set; }
      public ObservableCollection<Language> Languages { get; set; }

      public GradeSystem SelectedBoulderingGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> BoulderingGradingSystems { get; set; }

      public GradeSystem SelectedSportRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> SportRouteGradingSystems { get; set; }

      public GradeSystem SelectedTradRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> TradRouteGradingSystems { get; set; }

      public SettingsViewModel() {
         Title = VmTitle;
      }

      public void OnSelectedLanguageChanged() {

      }

      public void OnSelectedBoulderingGradingSystemChanged() {

      }

      public void OnSelectedSportRouteGradingSystemChanged() {

      }

      public void OnSelectedTradRouteGradingSystemChanged() {

      }

      public override void OnNavigatedTo(params object[] parameters) {
         base.OnNavigatedTo(parameters);

         Task.Run(async () => await InitializeViewModel());
      }

      private async Task InitializeViewModel() {
         try {
            Languages = await Client.LanguagesClient.ListAsync();

            var gradeSystems = await Client.GradesClient.ListAsync();
            BoulderingGradingSystems = gradeSystems.First(gs => gs.RouteType == 1).GradeSystems;
            SportRouteGradingSystems = gradeSystems.First(gs => gs.RouteType == 2).GradeSystems;
            TradRouteGradingSystems = gradeSystems.First(gs => gs.RouteType == 4).GradeSystems;
         } catch (ApiCallException ex) {
            await Errors.HandleApiCallExceptionAsync(ex);
         }
      }
   }
}