using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Core.API {
   public class RestApiClientSettings {
      public string BaseUrl { get; set; }

      public string Token { get; set; }
      public string RefreshToken { get; set; }
      public string Username { get; set; }
   }
}
