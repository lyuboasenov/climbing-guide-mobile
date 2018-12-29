using Climbing.Guide.Forms.Validations.Rules;
using System;

namespace Climbing.Guide.Forms.Validations {
   public static class IValidatableExtension {
      public static void AddRule(this IValidatable self, string key, params IRule[] rules) {
         if (null == self) {
            throw new ArgumentNullException(nameof(self));
         }

         if (null == self.ValidationRules) {
            throw new ArgumentNullException(nameof(self.ValidationRules));
         }

         self.ValidationRules.Add(key, rules);
      }
   }
}
