using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;

      public string Username { get; set; }
      public string Password { get; set; }
      public string ConfirmPassword { get; set; }

      public ICommand SignupCommand { get; private set; }
      public ICommand LoginCommand { get; private set; }

      public SignupViewModel() {
         Title = VmTitle;
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         SignupCommand = new Command(async () => await SignUp(), CanSignUp);
         LoginCommand = new Command(async () => await Login());
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         ((Command)SignupCommand).ChangeCanExecute();
      }

      private static System.ComponentModel.DataAnnotations.EmailAddressAttribute EmailAddressAttribute { get; } = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
      private bool CanSignUp() {
         var isEmailValid = !string.IsNullOrEmpty(Username) && EmailAddressAttribute.IsValid(Username);
         var isPasswordValid = !string.IsNullOrEmpty(Password) &&
            Password.Trim().Length > 8 &&
            Password.Any(char.IsDigit) &&
            Password.Any(char.IsLetter);
         var isPasswordsMatch = !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(ConfirmPassword) &&
            string.CompareOrdinal(Password.Trim(), ConfirmPassword) == 0;

         var validationErrors = new List<string>();
         if (!string.IsNullOrEmpty(Username) && !isEmailValid) {
            validationErrors.Add(Resources.Strings.User.Username_Validation_Error);
         }
         if (!string.IsNullOrEmpty(Password) && !isPasswordValid) {
            validationErrors.Add(Resources.Strings.User.Password_Validation_Error);
         }
         if (!string.IsNullOrEmpty(ConfirmPassword) && !isPasswordsMatch) {
            validationErrors.Add(Resources.Strings.User.Confirm_Password_Validation_Error);
         }
         ValidationErrors = validationErrors;

         return isEmailValid && isPasswordValid && isPasswordsMatch;
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