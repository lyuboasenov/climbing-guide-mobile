using Alat.Validation;
using Alat.Validation.Rules;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Services.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using INavigation = Climbing.Guide.Forms.Services.Navigation.INavigation;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LoginViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.User.Login_Title;
      public static INavigationRequest GetNavigationRequest(INavigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.User.LoginView));
      }

      public IValidationContext ValidationContext { get; }

      public string Username { get; set; }
      public string Password { get; set; }

      public ICommand LoginCommand { get; private set; }
      public ICommand SignupCommand { get; private set; }

      private IApiClient Client { get; }
      private IExceptionHandler Errors { get; }
      private INavigation Navigation { get; }
      private IAlerts Alerts { get; }

      public LoginViewModel(IApiClient client,
         IExceptionHandler errors,
         INavigation navigation, 
         IAlerts alerts,
         ValidationContextFactory validationContextFactory) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Alerts = alerts;

         Title = VmTitle;

         InitializeCommands();

         ValidationContext = validationContextFactory.GetContextFor(this, true);
      }

      public void InitializeValidationRules(IValidationContext context) {
         context.AddRule<LoginViewModel, string>(t => t.Username,
            new EmailRule(Resources.Strings.User.Username_Validation_Error));
         context.AddRule<LoginViewModel, string>(t => t.Password,
            new CustomRule(Resources.Strings.User.Password_Validation_Error,
               (key, value) => {
                  var password = value as string;
                  return !string.IsNullOrEmpty(password) &&
                  password.Trim().Length > 8 &&
                  password.Any(char.IsDigit) &&
                  password.Any(char.IsLetter);
               }));
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
            await Alerts.DisplayAlertAsync(Resources.Strings.User.Login_Invalid_Title,
               Resources.Strings.User.Login_Invalid_Message, Resources.Strings.Main.Ok);
         } else {
            await Navigation.NavigateAsync(
               HomeViewModel.GetNavigationRequest(Navigation));
         }
      }

      private void InitializeCommands() {
         LoginCommand = new Command(async () => await Login(), () => ValidationContext.IsValid);
         SignupCommand = new Command(async () => await Signup());
      }

      private async Task Signup() {
         await Navigation.NavigateAsync(
            SignupViewModel.GetNavigationRequest(Navigation));
      }

      public void OnValidationContextChanged() {
         // Raise validation context property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationContext));

         // Update can execute of the login command
         (LoginCommand as Command).ChangeCanExecute();
      }
   }
}