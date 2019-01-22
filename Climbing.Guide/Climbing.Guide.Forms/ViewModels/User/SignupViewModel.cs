using Alat.Validation;
using Alat.Validation.Rules;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;
      public static NavigationRequest GetNavigationRequest(Navigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.User.SignupView));
      }

      public IValidationContext ValidationContext { get; }

      public string Username { get; set; }
      public string Password { get; set; }
      public string ConfirmPassword { get; set; }

      public ICommand SignupCommand { get; private set; }
      public ICommand LoginCommand { get; private set; }

      protected IExceptionHandler Errors { get; }

      private IApiClient Client { get; }
      private Navigation Navigation { get; }

      public SignupViewModel(IApiClient client,
         IExceptionHandler errors,
         Navigation navigation,
         ValidationContextFactory validationContextFactory) {
         Client = client;
         Errors = errors;
         Navigation = navigation;

         Title = VmTitle;

         InitializeCommands();

         // ValidationContext should be initialized after all other initialization is done
         ValidationContext = validationContextFactory.GetContextFor(this, true);
      }

      public void OnValidationContextChanged() {
         // Raise validation context property changed in order to update validation errors
         RaisePropertyChanged(nameof(ValidationContext));

         (SignupCommand as Command).ChangeCanExecute();
      }

      public void InitializeValidationRules(IValidationContext context) {
         context.AddRule<SignupViewModel, string>(t => t.Username,
            new RequiredRule(Resources.Strings.User.Username_Validation_Error),
            new EmailRule(Resources.Strings.User.Username_Validation_Error));

         var passwordComparer = new CompareRule(Resources.Strings.User.Confirm_Password_Validation_Error);
         context.AddRule<SignupViewModel, string>(t => t.Password,
            new CustomRule(Resources.Strings.User.Password_Validation_Error,
               (key, value) => {
                  var password = value as string;
                  return !string.IsNullOrEmpty(password) &&
                  password.Trim().Length > 8 &&
                  password.Any(char.IsDigit) &&
                  password.Any(char.IsLetter);
               }),
            passwordComparer);
         context.AddRule<SignupViewModel, string>(t => t.ConfirmPassword,
            passwordComparer);
      }

      private void InitializeCommands() {
         SignupCommand = new Command(async () => await SignUp(), () => ValidationContext.IsValid);
         LoginCommand = new Command(async () => await NavigateToLogin());
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
         await Navigation.NavigateAsync(
            LoginViewModel.GetNavigationRequest(Navigation));
      }
   }
}