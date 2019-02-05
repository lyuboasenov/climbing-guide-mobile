using Alat.Caching;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Forms.Queries;
using Climbing.Guide.Forms.Services.Navigation;
using Climbing.Guide.Forms.Services.Preferences;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels.Settings {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SettingsViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Settings.Settings_Title;

      public static INavigationRequest GetNavigationRequest(INavigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.Settings.SettingsView));
      }

      public Language SelectedLanguage { get; set; }
      public ObservableCollection<Language> Languages { get; set; }

      public GradeSystem SelectedBoulderingGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> BoulderingGradingSystems { get; set; }

      public GradeSystem SelectedSportRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> SportRouteGradingSystems { get; set; }

      public GradeSystem SelectedTradRouteGradingSystem { get; set; }
      public ObservableCollection<GradeSystem> TradRouteGradingSystems { get; set; }

      public System.Windows.Input.ICommand ClearCacheCommand { get; private set; }

      public long CacheSize { get; set; }

      private IPreferences Preferences { get; }
      private ICache Cache { get; }
      private IQueryFactory QueryFactory { get; }

      public SettingsViewModel(IPreferences preferences, ICache cache, IQueryFactory queryFactory) {
         Preferences = preferences;
         Cache = cache;
         QueryFactory = queryFactory;

         Title = VmTitle;

         Languages = new ObservableCollection<Language>();
         BoulderingGradingSystems = new ObservableCollection<GradeSystem>();
         SportRouteGradingSystems = new ObservableCollection<GradeSystem>();
         TradRouteGradingSystems = new ObservableCollection<GradeSystem>();

         InitializeCommands();
      }

      public void OnSelectedLanguageChanged() {
         Preferences.LanguageCode = SelectedLanguage.Code;
      }

      public void OnSelectedBoulderingGradingSystemChanged() {
         Preferences.BoulderingGradeSystem = SelectedBoulderingGradingSystem.Id.Value;
      }

      public void OnSelectedSportRouteGradingSystemChanged() {
         Preferences.SportRouteGradeSystem = SelectedSportRouteGradingSystem.Id.Value;
      }

      public void OnSelectedTradRouteGradingSystemChanged() {
         Preferences.TradRouteGradeSystem = SelectedTradRouteGradingSystem.Id.Value;
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
         var languagesQuery = QueryFactory.GetQuery<LanguagesQuery>();
         var languages = await languagesQuery.GetResultAsync();

         Languages.Clear();
         foreach (var language in languages) {
            Languages.Add(language);
         }

         SelectedLanguage = Languages.First(l =>
            l.Code.Equals(Preferences.LanguageCode, StringComparison.Ordinal));

         var gradeSystemsQuery = QueryFactory.GetQuery<GradeSystemsQuery>();
         var gradeSystems = await gradeSystemsQuery.GetResultAsync();

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
            BoulderingGradingSystems.First(gs => gs.Id.Value == Preferences.BoulderingGradeSystem);
         SelectedSportRouteGradingSystem =
            SportRouteGradingSystems.First(gs => gs.Id.Value == Preferences.SportRouteGradeSystem);
         SelectedTradRouteGradingSystem =
            TradRouteGradingSystems.First(gs => gs.Id.Value == Preferences.TradRouteGradeSystem);

         CacheSize = Cache.GetCacheSize();
      }
   }
}