using Climbing.Guide.Forms.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Forms.ViewModels.Guide {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ManageRegionViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.Guide.Manage_Region_Title;

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
         SaveCommand = new Command(async () => await Save(), CanSave);
         CancelCommand = new Command(async () => await GoBack());
      }

      public async override Task OnNavigatedToAsync(params object[] parameters) {
         var progressService = GetService<IProgressService>();
         try {
            //await progressService.ShowLoadingIndicatorAsync();
            await base.OnNavigatedToAsync(parameters);
         } finally {
            //await progressService.HideLoadingIndicatorAsync();
         }
      }

      protected async override Task InitializeViewModel() {
         await base.InitializeViewModel();

         Location position = null;
         try {
            position = await Geolocation.GetLastKnownLocationAsync();
            if (null == position) {
               position = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30)));
            }
         } catch (FeatureNotSupportedException fnsEx) {
            await Errors.HandleAsync(fnsEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (PermissionException pEx) {
            await Errors.HandleAsync(pEx,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         } catch (Exception ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Permission_Exception_Format,
               Resources.Strings.Main.Location_Permissino);
         }

         if (null != position) {
            Location = MapSpan.FromCenterAndRadius(
            new Position(position.Latitude, position.Longitude),
            new Distance(10000));
         }
      }

      private async Task GoBack() {
         await Navigation.GoBackAsync();
      }

      private bool CanSave() {
         // TODO: validate form
         return true;
      }

      private async Task Save() {
         var payload = new Climbing.Guide.Api.Schemas.Region() {
            Name = Name,
            Info = Info,
            Restrictions = Restrictions,
            Latitude = (decimal)Location.Center.Latitude,
            Longitude = (decimal)Location.Center.Longitude,
            Size = (decimal)Location.Radius.Meters * 2
         };

         await Client.RegionsClient.CreateAsync(payload);
      }
   }
}