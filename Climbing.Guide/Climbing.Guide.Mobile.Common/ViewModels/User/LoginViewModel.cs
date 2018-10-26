﻿using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Mobile.Common.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LoginViewModel : BaseViewModel {
      public static string VmTitle { get; } = Resources.Strings.User.Login_Title;

      public string Username { get; set; }
      public string Password { get; set; }

      public LoginViewModel() {
         Title = VmTitle;

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
            //TODO
            //Navigation.UpdateNavigationContainer();
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
            //TODO
            //await CoreMethods.PushPageModel<SignupViewModel>();
         } catch (System.Exception ex) {
            GetService<IErrorService>().LogException(ex);
         }
      }
   }
}