using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Climbing.Guide.Forms.ViewModels.Settings {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SettingsViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Settings.Settings_Title;

      private IResourceService ResourceService { get; set; }
      private IPreferenceService PreferenceService { get; set; }
      private Caching.ICache Cache { get; set; }

      public Language SelectedLanguage { get; set; }
      public ObservableCollection<Language> Languages { get; set; }

      public GradeSystem SelectedBoulderingGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> BoulderingGradingSystems { get; set; }

      public GradeSystem SelectedSportRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> SportRouteGradingSystems { get; set; }

      public GradeSystem SelectedTradRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> TradRouteGradingSystems { get; set; }

      public ICommand ClearCacheCommand { get; private set; }

      public long CacheSize { get; set; }

      public SettingsViewModel(IResourceService resourceService, IPreferenceService preferenceService, Caching.ICache cache) {
         Title = VmTitle;

         ResourceService = resourceService;
         PreferenceService = preferenceService;
         Cache = cache;
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         ClearCacheCommand = new Xamarin.Forms.Command(ClearCache);
      }

      public void OnSelectedLanguageChanged() {
         PreferenceService.LanguageCode = SelectedLanguage.Code;
      }

      public void OnSelectedBoulderingGradingSystemChanged() {
         PreferenceService.BoulderingGradeSystem = SelectedBoulderingGradingSystem.Id.Value;
      }

      public void OnSelectedSportRouteGradingSystemChanged() {
         PreferenceService.SportRouteGradeSystem = SelectedSportRouteGradingSystem.Id.Value;
      }

      public void OnSelectedTradRouteGradingSystemChanged() {
         PreferenceService.TradRouteGradeSystem = SelectedTradRouteGradingSystem.Id.Value;
      }

      public override void OnNavigatedTo(params object[] parameters) {
         base.OnNavigatedTo(parameters);

         GetService<ITaskRunner>().Run(async () => await InitializeViewModel());
      }

      private void ClearCache() {
         Cache.Invalidate();
         CacheSize = Cache.GetCacheSize();
      }

      private async Task InitializeViewModel() {
         try {
            Languages = await ResourceService.GetLanguagesAsync();

            SelectedLanguage = Languages.First(l => l.Code.Equals(PreferenceService.LanguageCode, StringComparison.Ordinal));

            var gradeSystems = await ResourceService.GetGradeSystemsAsync();
            BoulderingGradingSystems = gradeSystems.First(gs => gs.RouteType == 1).GradeSystems;
            SportRouteGradingSystems = gradeSystems.First(gs => gs.RouteType == 2).GradeSystems;
            TradRouteGradingSystems = gradeSystems.First(gs => gs.RouteType == 4).GradeSystems;

            SelectedBoulderingGradingSystem = 
               BoulderingGradingSystems.First(gs => gs.Id.Value == PreferenceService.BoulderingGradeSystem);
            SelectedSportRouteGradingSystem =
               SportRouteGradingSystems.First(gs => gs.Id.Value == PreferenceService.SportRouteGradeSystem);
            SelectedTradRouteGradingSystem =
               TradRouteGradingSystems.First(gs => gs.Id.Value == PreferenceService.TradRouteGradeSystem);

            CacheSize = Cache.GetCacheSize();
         } catch (ApiCallException ex) {
            await Errors.HandleApiCallExceptionAsync(ex);
         } catch (Exception ex) {

         }
      }
   }
}