using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Validations.Rules;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LoginViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Login_Title;

      private IApiClient Client { get; }
      private IExceptionHandler Errors { get; }
      private Services.INavigation Navigation { get; }
      private IAlerts Alerts { get; }
      private IEvents Events { get; }

      public ICommand LoginCommand { get; }
      public ICommand SignupCommand { get; }

      public string Username { get; set; }
      public string Password { get; set; }

      public LoginViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation, 
         IAlerts alerts, 
         IEvents events) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Alerts = alerts;
         Events = events;

         Title = VmTitle;

         LoginCommand = new Command(async () => await Login(), () => !HasValidationErrors);
         SignupCommand = new Command(async () => await Signup());
      }

      private async Task Login() {
         bool success = false;
         try {
            success = await Client.AuthenticationManager.LoginAsync(Username, Password);
         } catch (ApiCallException ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Communication_Error_Message,
               Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
         }

         if (!success) {
            await Alerts.DisplayAlertAsync(Resources.Strings.User.Login_Invalid_Title, Resources.Strings.User.Login_Invalid_Message, Resources.Strings.Main.Ok);
         } else {
            Events.GetEvent<Events.ShellMenuInvalidatedEvent>().Publish();
            await Navigation.NavigateAsync(Navigation.GetShellNavigationUri(nameof(Views.HomeView)));
         }
      }

      protected override void InitializeValidationRules() {
         base.InitializeValidationRules();
         AddValidationRule(nameof(Username), new EmailRule(Resources.Strings.User.Username_Validation_Error));
         AddValidationRule(nameof(Password),
            new CustomRule(Resources.Strings.User.Password_Validation_Error,
               (key, value) => {
                  var password = value as string;
                  return !string.IsNullOrEmpty(password) &&
                  password.Trim().Length > 8 &&
                  password.Any(char.IsDigit) &&
                  password.Any(char.IsLetter);
               }));
      }

      // Update can execute of the login command
      public override void OnPropertyChanged(string propertyName, object before, object after) {
         base.OnPropertyChanged(propertyName, before, after);

         if (null != LoginCommand) {
            (LoginCommand as Command).ChangeCanExecute();
         }
      }

      private async Task Signup() {
         var navigationResult = await Navigation.NavigateAsync(Navigation.GetShellNavigationUri(nameof(Views.User.SignupView)));
         if (!navigationResult.Result) {
            await Errors.HandleAsync(navigationResult.Exception,
               Resources.Strings.Main.Shell_Navigation_Error_Message, SignupViewModel.VmTitle);
         }
      }
   }
}