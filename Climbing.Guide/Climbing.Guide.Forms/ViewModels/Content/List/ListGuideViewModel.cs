using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Services.Alerts;
using Climbing.Guide.Forms.Services.Exceptions;
using Climbing.Guide.Forms.Services.Media;
using Climbing.Guide.Forms.Services.Navigation;
using Climbing.Guide.Forms.Services.Progress;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using INavigation = Climbing.Guide.Forms.Services.Navigation.INavigation;

namespace Climbing.Guide.Forms.ViewModels.Content.List {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ListGuideViewModel : BaseGuideViewModel<ListGuideViewModel.Parameters> {
      public static string VmTitle { get; } = Resources.Strings.Guide.Guide_Title;

      public static INavigationRequest GetNavigationRequest(INavigation navigation) {
         return GetNavigationRequest(navigation, new Parameters());
      }

      public static INavigationRequest GetNavigationRequest(INavigation navigation, Parameters parameters) {
         return navigation.GetNavigationRequest(nameof(Views.Content.List.ListGuideView), parameters);
      }

      public ICommand TraverseBackCommand { get; private set; }
      public ICommand ItemTappedCommand { get; private set; }
      public ICommand AddItemCommand { get; private set; }

      private IProgress Progress { get; }

      public ListGuideViewModel(IApiClient client,
         IExceptionHandler exceptionHandler,
         INavigation navigation,
         IAlerts alerts,
         IMedia media,
         IProgress progress) :
         base(client, exceptionHandler, media, alerts, navigation) {
         Progress = progress;

         Title = VmTitle;

         InitializeCommands();
      }

      protected async override Task OnNavigatedToAsync() {
         await base.OnNavigatedToAsync();
         await InitializeData(new Parameters());

         Area parentArea = null;
         if (TraversalStack.Count > 0) {
            parentArea = TraversalStack.Pop();
            TraversalPath.Remove(parentArea);
         }

         await TraverseToAsync(parentArea);
      }

      protected async override Task OnNavigatedToAsync(Parameters parameters) {
         await base.OnNavigatedToAsync(parameters);
         await InitializeData(parameters);

         Area parentArea = null;
         if (TraversalStack.Count > 0) {
            parentArea = TraversalStack.Pop();
            TraversalPath.Remove(parentArea);
         }

         await TraverseToAsync(parentArea);
      }

      protected async override Task TraverseToAsync(Area area) {
         using (var loading = await Progress.CreateLoadingSessionAsync()) {
            await base.TraverseToAsync(area);
            (TraverseBackCommand as Command)?.ChangeCanExecute();
         }
      }

      private void InitializeCommands() {
         TraverseBackCommand = new Command(async () => await OnTraverseBackAsync(), () => TraversalStack.Count > 1);
         ItemTappedCommand = new Command<object>(async (item) => await OnItemTappedAsync(item));
         AddItemCommand = new Command(async () => await OnAddAsync());
      }

      private Task InitializeData(Parameters parameters) {
         try {
            if (parameters.TraversalPath != null) {
               foreach(var area in parameters.TraversalPath) {
                  TraversalStack.Push(area);
                  TraversalPath.Add(area);
               }
            }
            return Task.CompletedTask;
         } catch (Exception ex) {
            return ExceptionHandler.HandleAsync(ex);
         }
      }

      private async Task OnTraverseBackAsync() {
         await TraverseBackAsync();
         (TraverseBackCommand as Command)?.ChangeCanExecute();
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
         await Navigation.NavigateAsync(
            DisplayRouteViewModel.GetNavigationRequest(
               Navigation,
               new DisplayRouteViewModel.Parameters() {
                  Route = route
               }));
      }

      public class Parameters {
         public IEnumerable<Area> TraversalPath { get; set; }
      }
   }
}