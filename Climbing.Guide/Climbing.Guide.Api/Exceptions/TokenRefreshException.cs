using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Api.Exceptions {
   public class TokenRefreshException : Exception {
      public TokenRefreshException() { }
      public TokenRefreshException(string message, Exception innerException) :base(message, innerException) { }

   }
}
