using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using System;
using Climbing.Guide.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Climbing.Guide.Forms.Services.Navigation;
using Alat.Caching;

namespace Climbing.Guide.Forms.ViewModels.Settings {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SettingsViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Settings.Settings_Title;
      public static NavigationRequest GetNavigationRequest(Navigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.Settings.SettingsView));
      }

      private IExceptionHandler Errors { get; }
      private Resource ResourceService { get; set; }
      private Preferences PreferenceService { get; set; }
      private ICache Cache { get; set; }

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

      public SettingsViewModel(IExceptionHandler errors,
         Resource resourceService,
         Preferences preferenceService,
         ICache cache) {
         Errors = errors;
         ResourceService = resourceService;
         PreferenceService = preferenceService;
         Cache = cache;

         Title = VmTitle;

         Languages = new ObservableCollection<Language>();
         BoulderingGradingSystems = new ObservableCollection<GradeSystem>();
         SportRouteGradingSystems = new ObservableCollection<GradeSystem>();
         TradRouteGradingSystems = new ObservableCollection<GradeSystem>();

         InitializeCommands();
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

      protected async override Task OnNavigatedToAsync() {
         await base.OnNavigatedToAsync();
         await InitializeViewModel();
      }

      private void InitializeCommands() {
         ClearCacheCommand = new Xamarin.Forms.Command(ClearCache);
      }

      private void ClearCache() {
         Cache.RemoveAll();
         CacheSize = Cache.GetCacheSize();
      }

      private async Task InitializeViewModel() {
         try {
            var languages = await ResourceService.GetLanguagesAsync();
            Languages.Clear();
            foreach (var language in languages) {
               Languages.Add(language);
            }

            SelectedLanguage = Languages.First(l => l.Code.Equals(PreferenceService.LanguageCode, StringComparison.Ordinal));

            var gradeSystems = await ResourceService.GetGradeSystemsAsync();

            BoulderingGradingSystems.Clear();
            foreach (var system in gradeSystems.First(gs => gs.RouteType == 1).GradeSystems) {
               BoulderingGradingSystems.Add(system);
            }
            SportRouteGradingSystems.Clear();
            foreach(var system in gradeSystems.First(gs => gs.RouteType == 2).GradeSystems) {
               SportRouteGradingSystems.Add(system);
            }
            TradRouteGradingSystems.Clear();
            foreach(var system in gradeSystems.First(gs => gs.RouteType == 4).GradeSystems) {
               TradRouteGradingSystems.Add(system);
            }

            SelectedBoulderingGradingSystem = 
               BoulderingGradingSystems.First(gs => gs.Id.Value == PreferenceService.BoulderingGradeSystem);
            SelectedSportRouteGradingSystem =
               SportRouteGradingSystems.First(gs => gs.Id.Value == PreferenceService.SportRouteGradeSystem);
            SelectedTradRouteGradingSystem =
               TradRouteGradingSystems.First(gs => gs.Id.Value == PreferenceService.TradRouteGradeSystem);

            CacheSize = Cache.GetCacheSize();
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Communication_Error_Message,
               Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
         } catch (Exception ex) {
            await Errors.HandleAsync(ex);
         }
      }
   }
}