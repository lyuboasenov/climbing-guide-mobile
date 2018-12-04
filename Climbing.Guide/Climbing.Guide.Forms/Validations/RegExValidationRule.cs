using System.Text.RegularExpressions;

namespace Climbing.Guide.Forms.Validations {
   public class RegExValidationRule : IValidationRule {
      private string RegExPattern { get; set; }
      public string ErrorMessage { get; private set; }
      public bool IsValid { get; private set; }

      public RegExValidationRule(string regExPattern, string errorMessage) {
         RegExPattern = regExPattern;
         ErrorMessage = errorMessage;
      }

      public void Validate(string key, object value) {
         var strValue = value as string;

         IsValid = null != strValue && Regex.IsMatch(strValue, RegExPattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
      }
   }
}
