using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Forms.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LoginViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Login_Title;

      public string Username { get; set; }
      public string Password { get; set; }

      public LoginViewModel() {
         Title = VmTitle;

         Username = string.Empty;
         Password = string.Empty;

         LoginCommand = new Command(async () => await Login(), () => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
         SignupCommand = new Command(async () => await Signup());
      }

      public ICommand LoginCommand { get; }
      public ICommand SignupCommand { get; }
      
      private async Task Login() {
         bool success = false;
         try {
            success = await Client.LoginAsync(Username, Password);
         } catch (RestApiCallException ex) {
            await GetService<IErrorService>().HandleRestApiCallExceptionAsync(ex);
         }

         if (!success) {
            await GetService<IAlertService>().DisplayAlertAsync(Resources.Strings.User.Login_Invalid_Title, Resources.Strings.User.Login_Invalid_Message, Resources.Strings.Main.Ok);
         } else {
            GetService<IEventService>().GetEvent<Events.ShellMenuInalidated>().Publish();
            await NavigationService.NavigateAsync(NavigationService.GetShellNavigationUri(nameof(Views.HomeView)));
         }
      }

      // Update can execute of the login command
      public void OnPropertyChanged(string propertyName, object before, object after) {
         if(propertyName.CompareTo(nameof(Username)) == 0 ||
            propertyName.CompareTo(nameof(Password)) == 0) {
            (LoginCommand as Command).ChangeCanExecute();
         }
      }

      private async Task Signup() {
         try {
            await NavigationService.NavigateAsync(NavigationService.GetShellNavigationUri(nameof(Views.User.SignupView)));
         } catch (System.Exception ex) {
            GetService<IErrorService>().LogException(ex);
         }
      }
   }
}