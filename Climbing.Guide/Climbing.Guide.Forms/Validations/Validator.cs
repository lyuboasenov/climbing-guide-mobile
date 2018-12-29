using System;
using System.Collections.Generic;
using System.Linq;

namespace Climbing.Guide.Forms.Validations {
   public class Validator : IValidator {

      public void Validate(IValidatable target, string key, object value) {

         var validationRules = target.ValidationRules;

         if (null == validationRules ||
            validationRules.Count == 0) {
            throw new ArgumentNullException(nameof(target.ValidationRules));
         }

         if (validationRules.ContainsKey(key)) {
            target.ValidationErrors.Remove(key);
            var errors = new List<string>();

            foreach (var validationRule in validationRules[key]) {
               validationRule.Validate(key, value);
               if (!validationRule.IsValid) {
                  errors.Add(validationRule.ErrorMessage);
               }
            }

            if (errors.Count > 0) {
               target.ValidationErrors[key] = errors;
            }
         }

         UpdateIsValid(target);
      }

      private void UpdateIsValid(IValidatable target) {
         var result = false;
         if (null != target.ValidationRules) {
            var invalidRules = target.ValidationRules.Values.Sum(vrs => vrs.Count(vr => !vr.IsValid));

            result = invalidRules > 0;
         }

         target.IsValid = result;
      }
   }
}
