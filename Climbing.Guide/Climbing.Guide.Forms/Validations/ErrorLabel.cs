using Climbing.Guide.Forms.Validations;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Validations {
   public class ErrorLabel : Label {

      private static ValidationErrorConverter ValidationErrorConverter { get; } = new ValidationErrorConverter();

      public static readonly BindableProperty ErrorKeyProperty =
         BindableProperty.Create(nameof(ErrorKey), typeof(string), typeof(ErrorLabel), null, propertyChanged: OnErrorKeyPropertyChanged);

      public string ErrorKey {
         get { return (string)GetValue(ErrorKeyProperty); }
         set { SetValue(ErrorKeyProperty, value); }
      }

      private static void OnErrorKeyPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
         ((ErrorLabel)bindable).OnErrorKeyChanged(bindable, oldValue, newValue);
      }

      private void OnErrorKeyChanged(BindableObject bindable, object oldValue, object newValue) {
         var textBinding = new Binding("ValidationErrors", converter: ValidationErrorConverter, converterParameter: newValue);
         SetBinding(TextProperty, textBinding);
      }
   }
}
