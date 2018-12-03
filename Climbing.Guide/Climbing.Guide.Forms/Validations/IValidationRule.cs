using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Forms.Validations {
   public interface IValidationRule {
      void Validate(object value);
   }
}
