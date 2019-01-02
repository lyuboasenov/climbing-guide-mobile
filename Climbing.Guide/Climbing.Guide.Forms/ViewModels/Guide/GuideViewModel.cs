using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.Progress;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class GuideViewModel : BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      private IProgress Progress { get; }
      public ICommand TraverseBackCommand { get; private set; }
      public ICommand ItemTappedCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }

      public GuideViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation,
         IAlerts alerts, 
         IMedia media,
         IProgress progress) :
         base(client, errors, media, alerts, navigation) {
         Progress = progress;

         Title = VmTitle;

         InitializeCommands();
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         await base.OnNavigatedToAsync(parameters);
         await TraverseToAsync(null);
      }

      protected async override Task TraverseToAsync(Area parentArea) {
         using (var loading = await Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(parentArea);
            (TraverseBackCommand as Command).ChangeCanExecute();
         }
      }

      private void InitializeCommands() {
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalStack.Count > 1);
         ItemTappedCommand = new Command<object>(async (item) => { await OnItemTappedAsync(item); });
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private async Task OnTraverseBackAsync() {
         await TraverseBackAsync();
         (TraverseBackCommand as Command).ChangeCanExecute();
      }

      private Task OnAddAsync() {
         return AddItemAsync();
      }

      private async Task OnItemTappedAsync(object item) {
         if (item is Area) {
            await TraverseToAsync(item as Area);
         } else if (item is Route) {
            await RouteTappedAsync(item as Route);
         } else {
            throw new ArgumentException(nameof(item));
         }
      }

      private async Task RouteTappedAsync(Route route) {
         var navigationResult = await Navigation.NavigateAsync(
            Navigation.GetShellNavigationUri(nameof(Views.Routes.RouteView)),
            route);
         if (!navigationResult.Result) {
            await Errors.HandleAsync(navigationResult.Exception,
               Resources.Strings.Routes.Route_View_Error_Message, route.Name);
         }
      }
   }
}