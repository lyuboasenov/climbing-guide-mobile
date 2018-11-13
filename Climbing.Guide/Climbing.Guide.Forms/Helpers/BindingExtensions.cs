using System;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Helpers {
   public static class BindingExtensions {
      public static BindingBase Clone(this BindingBase self) {
         Binding binding = self as Binding;
         if (null != binding) {
            return new Binding(binding.Path, binding.Mode) {
               Converter = binding.Converter,
               ConverterParameter = binding.ConverterParameter,
               StringFormat = binding.StringFormat,
               Source = binding.Source,
               UpdateSourceEventName = binding.UpdateSourceEventName,
               TargetNullValue = binding.TargetNullValue,
               FallbackValue = binding.FallbackValue,
            };
         } else {
            throw new ArgumentException("Unknown binding type.");
         }
      }
   }
}
