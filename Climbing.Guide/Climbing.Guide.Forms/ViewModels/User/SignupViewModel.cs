using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Validations;
using Climbing.Guide.Forms.Validations.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;

      public IDictionary<string, IEnumerable<string>> ValidationErrors { get; } = new Dictionary<string, IEnumerable<string>>();
      public IDictionary<string, IEnumerable<IRule>> ValidationRules { get; } = new Dictionary<string, IEnumerable<IRule>>();
      public bool IsValid { get; set; }

      public string Username { get; set; }
      public string Password { get; set; }
      public string ConfirmPassword { get; set; }

      public ICommand SignupCommand { get; private set; }
      public ICommand LoginCommand { get; private set; }

      protected IExceptionHandler Errors { get; }

      private IApiClient Client { get; }
      private Services.INavigation Navigation { get; }
      private IValidator Validator { get; }
      private bool IsInitialized { get; } = false;

      public SignupViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation,
         IValidator validator) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Title = VmTitle;
         Validator = validator;

         InitializeValidationRules();
         InitializeCommands();

         IsInitialized = true;
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         if (IsInitialized) {
            Validator.Validate(this, propertyName, after);
            // Raise validation errors property changed in order to update validation errors
            RaisePropertyChanged(nameof(ValidationErrors));

            (SignupCommand as Command).ChangeCanExecute();
         }
      }

      private void InitializeCommands() {
         SignupCommand = new Command(async () => await SignUp(), () => IsValid);
         LoginCommand = new Command(async () => await NavigateToLogin());
      }

      private void InitializeValidationRules() {
         this.AddRule(nameof(Username), 
            new RequiredRule(Resources.Strings.User.Username_Validation_Error),
            new EmailRule(Resources.Strings.User.Username_Validation_Error));

         var passwordComparer = new CompareRule(Resources.Strings.User.Confirm_Password_Validation_Error);
         this.AddRule(nameof(Password),
            new CustomRule(Resources.Strings.User.Password_Validation_Error,
               (key, value) => {
                  var password = value as string;
                  return !string.IsNullOrEmpty(password) &&
                  password.Trim().Length > 8 &&
                  password.Any(char.IsDigit) &&
                  password.Any(char.IsLetter);
               }),
            passwordComparer);
         this.AddRule(nameof(ConfirmPassword),
            passwordComparer);
      }

      private async Task SignUp() {
         try {
            await Client.UsersClient.CreateAsync(new Climbing.Guide.Api.Schemas.User() {
               Username = Username,
               Email = Username,
               Password = Password
            });

            await NavigateToLogin();
         } catch(Climbing.Guide.Api.Schemas.ApiCallException ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Communication_Error_Message,
               Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
         }
      }

      private async Task NavigateToLogin() {
         await Navigation.NavigateAsync(Navigation.GetShellNavigationUri(nameof(Views.User.LoginView)));
      }
   }
}