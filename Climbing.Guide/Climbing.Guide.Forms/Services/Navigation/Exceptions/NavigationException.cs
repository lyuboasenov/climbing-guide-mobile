using System;

namespace Climbing.Guide.Forms.Services.Navigation.Exceptions {
   public class NavigationException : Exception {
      public NavigationRequest NavigationRequest { get; }

      public NavigationException() { }
      public NavigationException(string message) : base(message) { }
      public NavigationException(string message, Exception innerException) : base (message, innerException) { }
      public NavigationException(string message, Exception innerException, NavigationRequest request) : 
         base (message, innerException) {
         NavigationRequest = request;
      }
   }
}
