using Climbing.Guide.Mobile.Common.Resources;
using Climbing.Guide.Mobile.Common.Services;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SignupViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Signup_Title;

      public string Username { get; set; }
      public string Password { get; set; }
      public string ConfirmPassword { get; set; }

      public ICommand SignupCommand { get; private set; }
      public ICommand BackCommand { get; private set; }

      public SignupViewModel() {
         Title = VmTitle;
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         SignupCommand = new Command(SignUp, CanSignUp);
         BackCommand = new Command(GoBack);
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         ((Command)SignupCommand).ChangeCanExecute();
      }

      private bool CanSignUp(object arg) {
         // TODO: Add email validation
         return !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(ConfirmPassword) &&
            string.CompareOrdinal(Password, ConfirmPassword) == 0;
      }

      private void SignUp(object obj) {
         throw new NotImplementedException();
      }

      private void GoBack(object obj) {
         //TODO
         //CoreMethods.PopPageModel(true);
         //GetService<INavigationService>().SendBackButtonPressed();
      }
   }
}