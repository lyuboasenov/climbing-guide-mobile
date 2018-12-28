using Climbing.Guide.Forms.Validations.Rules;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class BaseViewModel : BindableBase, INavigationAware {

      private IDictionary<string, List<IRule>> ValidationRules { get; set; }
      public IDictionary<string, List<string>> ValidationErrors { get; set; }

      public BaseViewModel Parent { get; set; }
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public BaseViewModel() {
         InitializeCommands();
         InitializeValidationRules();
      }

      protected virtual void InitializeValidationRules() { }

      protected void AddValidationRule(string propertyName, IRule rule) {
         if (null == ValidationRules) {
            ValidationRules = new Dictionary<string, List<IRule>>();
         }

         if (!ValidationRules.ContainsKey(propertyName)) {
            ValidationRules.Add(propertyName, new List<IRule>());
         }

         ValidationRules[propertyName].Add(rule);
      }

      protected bool HasValidationErrors {
         get {
            var result = false;
            if (null != ValidationRules) {
               var invalidRules = ValidationRules.Values.Sum(vrs => vrs.Count(vr => !vr.IsValid));

               result = invalidRules > 0;
            }

            return result;
         }
      }

      protected virtual void InitializeCommands() { }

      protected virtual Task InitializeViewModel() {
         return Task.CompletedTask;
      }

      public void OnNavigatedFrom(INavigationParameters parameters) {
         OnNavigatedFromAsync(parameters.Select(p => p.Value));
      }

      public void OnNavigatedTo(INavigationParameters parameters) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
         OnNavigatedToAsync(parameters.Select(p => p.Value).ToArray());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }

      public void OnNavigatingTo(INavigationParameters parameters) {
         OnNavigatingToAsync(parameters.Select(p => p.Value));
      }

      public virtual Task OnNavigatedFromAsync(params object[] parameters) {
         return Task.CompletedTask;
      }

      public async virtual Task OnNavigatedToAsync(params object[] parameters) {
         await InitializeViewModel();
      }

      public virtual Task OnNavigatingToAsync(params object[] parameters) {
         return Task.CompletedTask;
      }

      public virtual void OnPropertyChanged(string propertyName, object before, object after) {
         ValidateRules(propertyName, after);
      }

      private void ValidateRules(string key, object value) {
         if (null != ValidationRules &&
            ValidationRules.ContainsKey(key)) {

            if (null == ValidationErrors) {
               ValidationErrors = new Dictionary<string, List<string>>();
            }
            ValidationErrors[key] = new List<string>();

            foreach (var validationRule in ValidationRules[key]) {
               validationRule.Validate(key, value);
               if (!validationRule.IsValid) {
                  ValidationErrors[key].Add(validationRule.ErrorMessage);
               }
            }

            // Raise validation errors property changed in order to update validation errors
            RaisePropertyChanged(nameof(ValidationErrors));
         }
      }
   }
}
