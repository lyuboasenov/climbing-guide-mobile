﻿using System;

namespace Climbing.Guide.Forms.Validations {
   public class CustomValidationRule : IValidationRule {
      public string ErrorMessage { get; private set; }
      public bool IsValid { get; private set; }
      private Func<string, object, bool> ValidateDelegate { get; set; }

      public CustomValidationRule(string errorMessage, Func<string, object, bool> validate) {
         ErrorMessage = errorMessage;
         ValidateDelegate = validate;
      }

      public void Validate(string key, object value) {
         IsValid = ValidateDelegate(key, value);
      }
   }
}
