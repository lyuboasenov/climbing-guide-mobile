using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Validations.Rules;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;

      private IApiClient Client { get; }
      protected IExceptionHandler Errors { get; }
      private Services.INavigation Navigation { get; }

      public string Username { get; set; }
      public string Password { get; set; }
      public string ConfirmPassword { get; set; }

      public ICommand SignupCommand { get; private set; }
      public ICommand LoginCommand { get; private set; }

      public SignupViewModel(IApiClient client,
         IExceptionHandler errors,
         Services.INavigation navigation) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Title = VmTitle;
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         SignupCommand = new Command(async () => await SignUp(), CanSignUp);
         LoginCommand = new Command(async () => await Login());
      }

      protected override void InitializeValidationRules() {
         base.InitializeValidationRules();
         AddValidationRule(nameof(Username), new RequiredRule(Resources.Strings.User.Username_Validation_Error));
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
         var passwordComparer = new CompareRule(Resources.Strings.User.Confirm_Password_Validation_Error);
         AddValidationRule(nameof(Password), passwordComparer);
         AddValidationRule(nameof(ConfirmPassword), passwordComparer);
      }

      public override void OnPropertyChanged(string propertyName, object before, object after) {
         base.OnPropertyChanged(propertyName, before, after);

         ((Command)SignupCommand).ChangeCanExecute();
      }

      private bool CanSignUp() {
         return HasValidationErrors;
      }

      private async Task SignUp() {
         try {
            await Client.UsersClient.CreateAsync(new Climbing.Guide.Api.Schemas.User() {
               Username = Username,
               Email = Username,
               Password = Password
            });

            await Login();
         } catch(Climbing.Guide.Api.Schemas.ApiCallException ex) {
            await Errors.HandleAsync(ex,
               Resources.Strings.Main.Communication_Error_Message,
               Resources.Strings.Main.Communication_Error_Message_Detailed_Format);
         }
      }

      private async Task Login() {
         await Navigation.NavigateAsync(Navigation.GetShellNavigationUri(nameof(Views.User.LoginView)));
      }
   }
}