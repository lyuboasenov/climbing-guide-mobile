using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Mobile.Common {
   internal class SessionManager {
      public static SessionManager Current { get; } = new SessionManager();

      public bool IsLoggedIn { get; private set; } = false;
   }
}
