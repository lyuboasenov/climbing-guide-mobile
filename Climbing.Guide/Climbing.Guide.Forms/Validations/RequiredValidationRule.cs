﻿using System;

namespace Climbing.Guide.Forms.Validations {
   public class RequiredValidationRule : IValidationRule {
      public string ErrorMessage { get; private set; }
      public bool IsValid { get; private set; }

      public RequiredValidationRule(string errorMessage) {
         ErrorMessage = errorMessage;
      }

      public void Validate(string key, object value) {
         var strValue = value as string;
         IsValid = null != strValue && !string.IsNullOrEmpty(strValue.Trim()) //value is string and is checked for whitespace only value
            || null == strValue && null != value; //if value is not string then only null check is applied
      }
   }
}
