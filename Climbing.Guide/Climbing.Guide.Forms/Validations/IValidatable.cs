using Climbing.Guide.Forms.Validations.Rules;
using System.Collections.Generic;

namespace Climbing.Guide.Forms.Validations {
   public interface IValidatable {
      IDictionary<string, IEnumerable<string>> ValidationErrors { get; }
      IDictionary<string, IEnumerable<IRule>> ValidationRules { get; }
      bool IsValid { get; set; }
   }
}
