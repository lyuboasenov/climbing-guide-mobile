﻿using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;
using Climbing.Guide.Forms.Services;
using Climbing.Guide.Forms.Validations;
using Climbing.Guide.Forms.Validations.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LoginViewModel : BaseViewModel, IValidatable {
      public static string VmTitle { get; } = Resources.Strings.User.Login_Title;

      public IDictionary<string, IEnumerable<string>> ValidationErrors { get; } = new Dictionary<string, IEnumerable<string>>();
      public IDictionary<string, IEnumerable<IRule>> ValidationRules { get; } = new Dictionary<string, IEnumerable<IRule>>();
      public bool IsValid { get; set; }

      public string Username { get; set; }
      public string Password { get; set; }

      public ICommand LoginCommand { get; }
      public ICommand SignupCommand { get; }

      private IApiClient Client { get; }
      private IExceptionHandler Errors { get; }
      private Navigation Navigation { get; }
      private Alerts Alerts { get; }
      private Services.Events Events { get; }
      private IValidator Validator { get; }
      private bool IsInitialized { get; } = false;

      public LoginViewModel(IApiClient client,
         IExceptionHandler errors,
         Navigation navigation, 
         Alerts alerts,
         Services.Events events,
         IValidator validator) {
         Client = client;
         Errors = errors;
         Navigation = navigation;
         Alerts = alerts;
         Events = events;
         Validator = validator;

         Title = VmTitle;

         LoginCommand = new Command(async () => await Login(), () => IsValid);
         SignupCommand = new Command(async () => await Signup());

         InitializeValidationRules();

         IsInitialized = true;
      }

      public void OnPropertyChanged(string propertyName, object before, object after) {
         if (IsInitialized) {
            Validator.Validate(this, propertyName, after);
            // Raise validation errors property changed in order to update validation errors
            RaisePropertyChanged(nameof(ValidationErrors));

            // Update can execute of the login command
            (LoginCommand as Command).ChangeCanExecute();
         }
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

      private void InitializeValidationRules() {
         this.AddRule(nameof(Username),
            new EmailRule(Resources.Strings.User.Username_Validation_Error));
         this.AddRule(nameof(Password),
            new CustomRule(Resources.Strings.User.Password_Validation_Error,
               (key, value) => {
                  var password = value as string;
                  return !string.IsNullOrEmpty(password) &&
                  password.Trim().Length > 8 &&
                  password.Any(char.IsDigit) &&
                  password.Any(char.IsLetter);
               }));
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