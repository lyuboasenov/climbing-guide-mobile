using System;
using System.Globalization;
using Climbing.Guide.Forms.Validations;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Validations {
   public class ErrorLabel : Label {

      private static ValidationErrorConverter ValidationErrorConverter { get; } = new ValidationErrorConverter();
      private static StringToVisibilityConverter StringToVisibilityConverter { get; } = new StringToVisibilityConverter();

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

         var visibilityBinding = new Binding("ValidationErrors", converter: StringToVisibilityConverter, converterParameter: newValue);
         SetBinding(IsVisibleProperty, visibilityBinding);
      }
   }
}
